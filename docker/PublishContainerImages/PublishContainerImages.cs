using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PublishContainerImages.Model;


namespace PublishContainerImages
{
    class PublishContainerImages
    {

        public static Action<string> WriteLog;
        public static Action<string> WriteError;

        public static string ContainerImagesDefinitionFilename = "containerImage.json";
        public static string BuiltImageMetadataFilename = "rendering-container-images.json";

        public static string TestConfigurationFilename = "testConfiguration.json";
        public static string TestParametersFilename = "testParameters.json";
        public static string LatestImagesFilename = "latestImages.txt";
        public static string TaggedImagesFilename = "taggedImages.txt";

        private const string LogFilename = "publishContainerImages.log";

        private const string StorageAccountName = "renderingapplications";
        private const string StoragContainerName = "batch-rendering-apps";


        private static StreamWriter _log;

        static void Main(string[] args)
        {
            using (var log = File.AppendText(LogFilename))
            {
              try
              {
                    _log = log;
                    _log.AutoFlush = true;
                    WriteLog = _writeLog;
                    WriteError = _writeError;
                    _writeLog($"Beginning New Publishing Run with args: {string.Join(", ", args)}");
                    
                    var storageKey = args[0];
                    var targetFolder = new DirectoryInfo(args[1]);
                    var includeAntecendents = TryParseAsBool(args[2]);
                    var includeDescendents = TryParseAsBool(args[3]);
                    var gitCommitSha = args[4];
                    var buildImages = TryParseAsBool(args[5]);
                    var pushImages = TryParseAsBool(args[6]);
                    var outputFullMetadataFile = TryParseAsBool(args[7]);

                    var blobContainer = _buildBlobClient(buildImages, StorageAccountName, storageKey, StoragContainerName); //NOTE blobContainer will be null if !buildImages

                    var containerImagePayload = DirectoryTraversal.BuildFullPayloadFromDirectoryTree(targetFolder, includeAntecendents, includeDescendents);

                    _writePrePublishLog(containerImagePayload);
                    var imageNumber = 1;
                    var latestImages = new List<string>();
                    var imagesWithShaTag = new List<string>();

                    if (buildImages)
                    {
                        foreach (var imageDef in containerImagePayload.Select(x => x.ContainerImageDefinition))
                        {
                            _writeLog($"Publishing #{imageNumber++} of {containerImagePayload.Count} - {imageDef.ContainerImage}");

                            dynamic blobProperties =
                                _getBlobUriWithSasTokenAndMD5(imageDef.InstallerFileBlob, blobContainer);

                            var localImageId = _buildImage(imageDef, blobProperties.blobSasToken);

                            string tag = ImageTagging._fetchImageTag(blobProperties.blobMD5, gitCommitSha);
                           
                            foreach (var imageTag in new []{ tag, "latest" })
                            {
                                DockerCommands._runDockerTag(imageDef, localImageId, imageTag);
                            }

                            var imageWithGeneratedTag = $"{imageDef.ContainerImage}:{tag}";
                            imagesWithShaTag.Add(imageWithGeneratedTag);
                            _writeLog($"Successfully built and tagged {imageWithGeneratedTag}");

                            if (pushImages)
                            { 
                                DockerCommands._runDockerPush(imageDef, tag);
                                _writeLog($"Successfully published {imageWithGeneratedTag}\n");
                            }

                            latestImages.Add($"{imageDef.ContainerImage}:latest");
                        }
                        OutputFileWriter._outputTestFiles(containerImagePayload, imagesWithShaTag);
                        OutputFileWriter._outputTaggedImagesFile(imagesWithShaTag);
                        OutputFileWriter._outputLatestImagesFile(latestImages);
                        OutputFileWriter._outputContainerImageMetadataFile(
                            containerImagePayload.Select(x => x.ContainerImageDefinition).ToList(), 
                            outputFullMetadataFile);
                        _writeLog($"Completed Publishing Successfully!\n\n");
                    }
                    else
                    {
                        latestImages = containerImagePayload.Select(x => $"{x.ContainerImageDefinition.ContainerImage}:latest").ToList();
                        OutputFileWriter._outputTestFiles(containerImagePayload, latestImages);
                        _writeLog($"Completed Generating Test Files!\n\n");
                    }
                }
            
              catch (Exception ex)
              {
                  _writeError("Fatal Exception: " + ex);
                  throw ex;
              }
            }
        }

        private static void _writePrePublishLog(List<ContainerImagePayload> containerImages)
        {
            var imageListLog = ($"Loaded {containerImages.Count} containerImages:\n");
            containerImages.ForEach(image => imageListLog += image.ContainerImageDefinition.ContainerImage + "\n");
            _writeLog(imageListLog);
        }
   
        private static void _writeLog(string log)
        {
            var logLine = string.Format(@"{0}: {1}", DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString(), log);
            _log.WriteLine(logLine);
            Console.WriteLine(log);
        }

        private static void _writeError(string error)
        {
            var logLine = string.Format(@"{0}: ERROR: {1}", DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString(), error);
            _log.WriteLine(logLine);
            Console.WriteLine($"ERROR: {error}");
        }

      

        private static string _buildImage(ContainerImageDefinition imageDefinition, string blobSasToken)
        {
            var dockerBuildOutput = DockerCommands._runDockerBuild(blobSasToken, imageDefinition);

            var localImageId = _imageIdFromDockerBuildOutput(dockerBuildOutput);

            return localImageId;
        }

        private static string _imageIdFromDockerBuildOutput(string[] output)
        {
            var keyLine = output.First(line => line.StartsWith("Successfully built "));

            var imageId = keyLine.Substring("Successfully built ".Length);

            return imageId;
        }

        private static CloudBlobContainer _buildBlobClient(bool buildImages, string storageAccountName, string storageKey, string containerName)
        {
            if (!buildImages)
            {
                return null;
            }

            var storageUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/");

            var storageClient = new CloudBlobClient(storageUri, new StorageCredentials(storageAccountName, storageKey));

            return storageClient.GetContainerReference(containerName);
        }

        private static dynamic _getBlobUriWithSasTokenAndMD5(string blobPath, CloudBlobContainer blobContainer)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                return new { blobSasToken = string.Empty, blobMD5 = string.Empty };
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

        private static bool TryParseAsBool(string toParse)
        {
            return bool.Parse(toParse.First().ToString().ToUpper() + toParse.Substring(1));
        }
    }
}
