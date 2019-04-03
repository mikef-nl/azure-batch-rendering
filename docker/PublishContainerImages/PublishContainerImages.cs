﻿using System;
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
        public static Action<string> WriteLog;
        public static Action<string> WriteError;

        private static StreamWriter _log;

        static void Main(string[] args)
        {
            //TODO move install scripts excluding dockerFiles (maybe these too?) inside project, remove duplication
            using (var log = File.AppendText("containerImagePublish.log"))
            {
              try
              {
                    _log = log;
                    _log.AutoFlush = true;
                    WriteLog = _writeLog;
                    WriteError = _writeError;
                    _writeLog($"Beginning New Publishing Run");

                    var storageKey = args[0];
                    var targetFolder = new DirectoryInfo(args[1]);
                    var buildImages = bool.Parse(args[2]);
                    var publishToRepo = bool.Parse(args[3]);
                    //var traversalMode = Enum.Parse(args[4]); TODO separate directory traversal modes - SingleImage, Descendents, Antecendents
                    //var validateImages = bool.Parse(args[5]); TODO link into Matts image validator, run validation after build + version tag and publish, only tag with 'latest' if validation succeeds
                    //var overwrite = bool.Parse(args[6]); TODO if false, only build and publish images which don't already exist for a given version tag, might need to check these on repo rather than local, if found local could maybe just redo the push?

                    var storageAccountName = "renderingapplications";
                    var containerName = "batch-rendering-apps";

                    var blobContainer = _buildBlobClient(buildImages, storageAccountName, storageKey, containerName); //NOTE blobContainer will be null if !buildImages

                    var containerImageDefs = DirectoryTraversal.ImageBuildOrderFromDirectoryTree(targetFolder, new List<ContainerImageDef>());

                    _writePrePublishLog(containerImageDefs);
                    var imageNumber = 1;

                    foreach (var imageDef in containerImageDefs)
                    {
                        _writeLog($"Publishing #{imageNumber++} of {containerImageDefs.Count} - {imageDef.containerImage}");

                        if (buildImages)
                        {
                            dynamic blobProperties =
                                _getBlobUriWithSasTokenAndMD5(imageDef.installerFileBlob, blobContainer);

                            var localImageId = _buildImage(imageDef, blobProperties.blobSasToken);

                            string[] tags = ImageTagging._fetchImageTags(blobProperties.blobMD5);

                            DockerCommands._runDockerTag(imageDef, localImageId, tags);

                            _writeLog($"Successfully built {imageDef.containerImage}:{tags.Last()}");

                            if (publishToRepo)
                            {
                                DockerCommands._runDockerPush(imageDef, tags);
                                _writeLog($"Successfully published {imageDef.containerImage}:{tags.Last()}");
                            }
                        }
                    }
                    _writeLog($"Completed Publishing Successfully!\n\n");
                
            }
            
              catch (Exception ex)
              {
                  _writeError("Fatal Exception: " + ex);
              }
            }
        }

        private static void _writePrePublishLog(List<ContainerImageDef> containerImages)
        {
            var imageListLog = ($"Loaded {containerImages.Count} containerImages:\n");
            containerImages.ForEach(image => imageListLog += image.containerImage + "\n");
            _writeLog(imageListLog);
        }
   
        private static void _writeLog(string log)
        {
            _log.WriteLine(@"{0}: {1}", DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString(), log);
            Console.WriteLine(log);
        }

        private static void _writeError(string error)
        {
            _log.WriteLine(@"{0}: ERROR: {1}", DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString(), error);
            Console.WriteLine("ERROR: " + error);
        }

        private static string _buildImage(ContainerImageDef imageDef, string blobSasToken)
        {
            var dockerBuildOutput = DockerCommands._runDockerBuild(blobSasToken, imageDef);

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
    }
}