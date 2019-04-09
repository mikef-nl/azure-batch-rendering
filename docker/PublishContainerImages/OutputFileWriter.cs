using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PublishContainerImages.Model;

namespace PublishContainerImages
{
    class OutputFileWriter
    {
        private const string RelativePathToTestsDir = "../Tests";
        private const string RelativePathToLatestImageDir = "../";

        public static void _outputLatestImagesFile(List<string> latestImages)
        {
            latestImages.ForEach(PublishContainerImages.WriteLog);
            var builtImagesFilePath = Path.Combine(OutputLatestImagesFilePath(), PublishContainerImages.LatestImagesFilename);
            File.WriteAllLines(builtImagesFilePath, latestImages);
        }

        public static void _outputTestFiles(List<ContainerImagePayload> payloads, List<string> builtImages)
        {
            var payloadsWithTests = _removeInvalidPayloads(payloads);
            var payloadsWithTestsAndImagesTagged = _updateTestConfigAndParametersWithTaggedImage(payloadsWithTests, builtImages);

            payloadsWithTestsAndImagesTagged.ForEach(payload =>
            {
                var parametersPath = Path.Combine(OutputTestPath(), payload.TestConfigurationDefinition.Parameters);
                var parametersJson = JsonSerializeObject(payload.TestParametersDefinition);
                FileInfo paramsFile = new FileInfo(parametersPath);
                Directory.CreateDirectory(paramsFile.DirectoryName);
                File.WriteAllText(parametersPath, parametersJson);
                PublishContainerImages.WriteLog($"Wrote parameters file at: {paramsFile.FullName}");
            });

            var testsConfiguration = new TestsDefinition
            {
                Tests = payloadsWithTestsAndImagesTagged.Select(payload =>
                {
                    var config = payload.TestConfigurationDefinition;
                    config.Parameters = Path.Combine(OutputTestPath(), config.Parameters).Replace("\\", "/");
                    return config;
                }).ToArray(),
                Images = new[]
                {
                    new MarketplaceImageDefinition
                    {
                        Offer = "microsoft-azure-batch",
                        OsType = "linux",
                        Publisher = "centos-container",
                        Sku = "7-5",
                        Version =  "latest",
                    }
                }
            };

            var testsConfigurationFilepath = Path.Combine(OutputTestPath(), PublishContainerImages.TestConfigurationFilename);
            var testsConfigurationJson = JsonSerializeObject(testsConfiguration);

            FileInfo configFile = new FileInfo(testsConfigurationFilepath);
            Directory.CreateDirectory(configFile.DirectoryName);
            File.WriteAllText(testsConfigurationFilepath, testsConfigurationJson);
            PublishContainerImages.WriteLog($"Wrote configuration file at: {configFile.FullName}");
        }

        private static List<ContainerImagePayload> _removeInvalidPayloads(List<ContainerImagePayload> payloads)
        {
            return payloads.Where(payload =>
                payload.TestConfigurationDefinition != null && payload.TestParametersDefinition != null).ToList();
        }

        private static List<ContainerImagePayload> _updateTestConfigAndParametersWithTaggedImage(List<ContainerImagePayload> payloads, List<string> publishedImages)
        {
            foreach (var containerImagePayload in payloads)
            {
                var publishedImageWithTag = publishedImages.Find(publishedImage =>
                {
                    var publishedImageWithoutTag = publishedImage.Split(':').First();
                    return publishedImageWithoutTag == containerImagePayload.ContainerImageDefinition.ContainerImage;
                });

                containerImagePayload.TestParametersDefinition.ContainerImage.Value = publishedImageWithTag;
                containerImagePayload.TestConfigurationDefinition.DockerImage = publishedImageWithTag;
            }

            return payloads;
        }

        private static string OutputTestPath()
        {
            var locationUri = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var executableDirectory = new FileInfo(locationUri.AbsolutePath).Directory;

            return Path.GetFullPath(Path.Combine(executableDirectory.FullName, RelativePathToTestsDir));
        }

        private static string OutputLatestImagesFilePath()
        {
            var locationUri = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var executableDirectory = new FileInfo(locationUri.AbsolutePath).Directory;

            return Path.GetFullPath(Path.Combine(executableDirectory.FullName, RelativePathToLatestImageDir));
        }

        private static string JsonSerializeObject(dynamic toSerialize)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(toSerialize, jsonSerializerSettings);
        }
    }
}
