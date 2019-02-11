using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;


namespace PublishContainerImages
{
    class PublishContainerImages
    {
        private static StreamWriter _log;

        static void Main(string[] args)
        {
            //TODO 
            // 1) move containerImages.json to directories alongside dockerFiles and support publishing by recursively traversing file tree from a target dir
            // 2) move install scripts excluding dockerFiles (maybe these too?) inside project, remove duplication
            // 3) unless -force flag is provided, skip building / pushing images which already exist for a given version tag,
            //    might need to check these on repo rather than local, if found local could maybe just redo the push?
            
            using (var log = File.AppendText("containerImagePublish.log"))
            {
              try
              {
                    _log = log;
                    _log.AutoFlush = true;

                    var storageKey = args[0];

                    var storageAccountName = "renderingapplications";
                    var containerName = "batch-rendering-apps";

                    var blobContainer = _buildBlobClient(storageAccountName, storageKey, containerName);

                    dynamic json = _readContainerImagesJson();
                    List<ContainerImageDef> containerImages = json.containerImages.ToObject<List<ContainerImageDef>>();

                    _writePrePublishLog(containerImages);
                    var imageNumber = 1;

                    foreach (var imageDef in containerImages)
                    {
                        _writeLog($"\nPublishing #{imageNumber++} of {containerImages.Count} - {imageDef.containerImage}");

                        dynamic blobProperties =
                            _getBlobUriWithSasTokenAndMD5(imageDef.installerFileBlob, blobContainer);

                        var localImageId = _buildImage(imageDef, blobProperties.blobSasToken);

                        string[] tags = _fetchImageTags(blobProperties.blobMD5);

                        _runDockerTag(imageDef, localImageId, tags);

                        _runDockerPush(imageDef, tags);

                        _writeLog($"Successfully published {imageDef.containerImage}:{tags.Last()}\n");
                    }
                    _writeLog($"Completed Publishing Successfully!");
                
            }
            
              catch (Exception ex)
              {
                  _writeLog("Fatal Exception: " + ex);
              }
            }
        }

        private static void _writePrePublishLog(List<ContainerImageDef> containerImages)
        {
            var imageListLog = ($"Loaded {containerImages.Count} containerImages:\n");
            containerImages.ForEach(image => imageListLog += image.containerImage + "\n");
            _writeLog(imageListLog);
        }

        private static string[] _fetchImageTags(string blobMd5)
        {
            var gitCommitSha = _getGitHeadShortSha();

            var sanitizedblobMd5 = _sanitizeBase64StringForDockerTag(blobMd5);

            var versionTag = $"git-{gitCommitSha}-blob-{sanitizedblobMd5}";

            var allTags = new[] { "latest", versionTag };

            return allTags;
        }

        private static string _sanitizeBase64StringForDockerTag(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return base64;
            }

            return base64.Replace('/', '_').Replace('+', '.').TrimEnd('=').Substring(0, 7);
        }

        private static string _getGitHeadShortSha()
        {
            var repoPath = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\..\\..\\.git";

            using (var repo = new Repository(repoPath))
            {
                var sha = repo.Commits.First().Sha;
                return sha.Substring(0, 7);
            }
        }

        private static dynamic _readContainerImagesJson()
        {
            string jsonStr;
            using (var streamReader = new StreamReader("containerImages.json"))
            {
                jsonStr = streamReader.ReadToEnd();
            }

            dynamic json = JsonConvert.DeserializeObject(jsonStr);

            return json;
        }

        private static string _buildImage(ContainerImageDef imageDef, string blobSasToken)
        {
            var dockerBuildOutput = _runDockerBuild(blobSasToken, imageDef);

            var localImageId = _imageIdFromDockerBuildOutput(dockerBuildOutput);

            return localImageId;
        }

        private static string _imageIdFromDockerBuildOutput(string[] output)
        {
            var keyLine = output.First(line => line.StartsWith("Successfully built "));

            var imageId = keyLine.Substring("Successfully built ".Length);

            return imageId;
        }

        private static void _runDockerTag(ContainerImageDef imageDef, string localImageId, string[] tags)
        {
            var _oneMinInMs = 1 * 1000 * 60;

            foreach (var tag in tags)
            {
                var commandLine = $"docker tag {localImageId} {imageDef.containerImage}:{tag}";

                _runCmdProcess(commandLine, _oneMinInMs);
            }
        }

        private static void _runDockerPush(ContainerImageDef imageDef, string[] tags)
        {
            var _twentyMinsInMs = 20 * 1000 * 60;

            foreach (var tag in tags)
            {
                var commandLine = $"docker push {imageDef.containerImage}:{tag}";

                _runCmdProcess(commandLine, _twentyMinsInMs);
            }
        }

        private static string[] _runDockerBuild(string blobSasToken, ContainerImageDef imageDef)
        {
            var _twentyMinsInMs = 20 * 1000 * 60;

            var dockerFileDirectory = "E:\\Code\\Github\\azure-batch-rendering\\docker\\" + imageDef.pathToDockerFile;

            var commandLine = $"docker build -m 4GB --build-arg INSTALLER_SAS=\"{blobSasToken}\" {dockerFileDirectory}";

            return _runCmdProcess(commandLine, _twentyMinsInMs);
        }

        private static dynamic _getBlobUriWithSasTokenAndMD5(string blobPath, CloudBlobContainer blobContainer)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                return new { blobSasToken = string.Empty, blobMD5 = string.Empty};
            }

            var blob = blobContainer.GetBlockBlobReference(blobPath);

            var sasConstraints =
                new SharedAccessBlobPolicy
                {
                    SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
                };

            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            blob.FetchAttributesAsync().GetAwaiter().GetResult();

            return new { blobSasToken = blob.Uri + sasBlobToken, blobMD5 = blob.Properties.ContentMD5 };
        }

        private static string[] _runCmdProcess(string commandLine, int timeoutInMs)
        {
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c " + commandLine,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            })
            {
                var output = new List<string>();

                process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _writeLog(e.Data);
                        output.Add(e.Data);
                    }
                };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _writeError(e.Data);
                        output.Add("ERROR: " + e.Data);
                    }
                };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit(timeoutInMs);
                process.WaitForExit();

                return output.ToArray();
            }
        }

        private static void _writeLog(string log)
        {
            Console.WriteLine(log);
            _log.WriteLine(@"{0}: {1}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), log);
        }

        private static void _writeError(string error)
        {
            Console.WriteLine("ERROR: " + error);
            _log.WriteLine(@"{0}: ERROR: {1}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), error);
        }

        private static CloudBlobContainer _buildBlobClient(string storageAccountName, string storageKey, string containerName)
        {
            var storageUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/");

            var storageClient = new CloudBlobClient(storageUri, new StorageCredentials(storageAccountName, storageKey));

            return storageClient.GetContainerReference(containerName);
        }

        private class ContainerImageDef
        {
            public ContainerImageDef()
            {}

            public  string containerImage;

            public string pathToDockerFile;

            public string installerFileBlob;
        }
    }
}
