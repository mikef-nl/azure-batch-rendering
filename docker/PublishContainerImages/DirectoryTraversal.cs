using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PublishContainerImages.Model;

namespace PublishContainerImages
{
    static class DirectoryTraversal
    {
        public static List<ContainerImagePayload> BuildFullPayloadFromDirectoryTree(DirectoryInfo targetDirectory, bool includeAntecendents, bool includeDescendents)
        {
            List<ContainerImagePayload> payloads = new List<ContainerImagePayload>();
            if (includeAntecendents)
            {
                payloads.AddRange(BuildPayloadForAntecendents(targetDirectory));
            }

            if (includeDescendents)
            {
                AddPayloadsForTargetAndDescendents(targetDirectory, payloads);
            }
            else
            {
                payloads.Add(PayloadForDirectory(targetDirectory));
            }

            return payloads;
        }

        private static List<ContainerImagePayload> BuildPayloadForAntecendents(DirectoryInfo root)
        {
            var topDirectory = _findTopDirectoryContainingContainerImageJson(root);
            var bottomDirectory = root;

            var directoryInfo = new List<DirectoryInfo>();
            do
            {
                bottomDirectory = bottomDirectory.Parent;
                directoryInfo.Add(bottomDirectory);
            } while (bottomDirectory != null && bottomDirectory.FullName != topDirectory.FullName);
            
            directoryInfo.Reverse(); //we need build order to have top level directories first

            return directoryInfo.Select(PayloadForDirectory).ToList();
        }

        private static List<ContainerImagePayload> AddPayloadsForTargetAndDescendents(DirectoryInfo root, List<ContainerImagePayload> payload)
        {
            payload.Add(PayloadForDirectory(root));

            var subDirs = root.GetDirectories();

            foreach (var dirInfo in subDirs)
            {
                AddPayloadsForTargetAndDescendents(dirInfo, payload);
            }

            return payload;
        }

        private static ContainerImagePayload PayloadForDirectory(DirectoryInfo directory)
        {
            var containerImageDefinition = ReadContainerImageDefinition(directory);
            var testConfigAndParams = TryReadTestConfigAndParams(directory);

           return new ContainerImagePayload
            {
                ContainerImageDefinition = containerImageDefinition,
                TestConfigurationDefinition = testConfigAndParams.Item1,
                TestParametersDefinition = testConfigAndParams.Item2,
            };
        }

        private static DirectoryInfo _findTopDirectoryContainingContainerImageJson(DirectoryInfo root)
        {
            DirectoryInfo lastDir = root;
            while (_fileInfoForContainerImageDefinition(root) != null)
            {
                lastDir = root;
                root = root.Parent;
            }

            return lastDir;
        }

        private static FileInfo _fileInfoForContainerImageDefinition(DirectoryInfo root)
        {
            FileInfo fileInfo = null;

            try
            {
                fileInfo = root.GetFiles(PublishContainerImages.ContainerImageDefinitionFilename).Single();
            }
            catch (UnauthorizedAccessException e)
            {
                PublishContainerImages.WriteError(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                PublishContainerImages.WriteError(e.Message);
            }
            catch (ArgumentNullException e)
            {
                PublishContainerImages.WriteError(e.Message);
            }
            catch (InvalidOperationException e)
            {
                PublishContainerImages.WriteError(e.Message);
            }

            return fileInfo;
        }

        private static dynamic _readJsonFileToDynamic(string filePath)
        {
            string jsonStr;
            using (var streamReader = new StreamReader(filePath))
            {
                jsonStr = streamReader.ReadToEnd();
            }

            dynamic json = JsonConvert.DeserializeObject(jsonStr);

            return json;
        }

        private static ContainerImageDefinition ReadContainerImageDefinition(DirectoryInfo directory)
        {
            var containerDefinitionFileInfo = _fileInfoForContainerImageDefinition(directory);

            if (containerDefinitionFileInfo != null)
            {
                ContainerImageDefinition containerImageDefinition;
                dynamic json = _readJsonFileToDynamic(containerDefinitionFileInfo.FullName);
                try
                {
                    containerImageDefinition = json.ToObject<ContainerImageDefinition>();
                }
                catch (JsonSerializationException ex)
                {
                    PublishContainerImages.WriteError(
                        $"Invalid Json read in file {containerDefinitionFileInfo}, Json was: {json}. Exception: {ex}");
                    throw;
                }
                return containerImageDefinition;
            }

            return null;
        }

        private static Tuple<TestConfigurationDefinition, TestParametersDefinition> TryReadTestConfigAndParams(DirectoryInfo directory)
        {
            var testConfigFiles = directory.GetFiles(PublishContainerImages.TestConfigurationFilename);
            var testParamsFiles = directory.GetFiles(PublishContainerImages.TestParametersFilename);

            if (!testConfigFiles.Any())
            {
                PublishContainerImages.WriteLog($"No Test will be run, testConfiguration.json not found in directory {directory}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            if (!testParamsFiles.Any())
            {
                PublishContainerImages.WriteLog($"No Test will be run, testParameters.json not found in directory {directory}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            if (testConfigFiles.Length > 1)
            {
                PublishContainerImages.WriteLog($"No test will be run, more than one TestConfiguration.json file found in {directory}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            if (testParamsFiles.Length > 1)
            {
                PublishContainerImages.WriteLog($"No test will be run, more than one TestParameters.json file found in {directory}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            var testConfigJson = _readJsonFileToDynamic(testConfigFiles.Single().FullName);
            TestConfigurationDefinition testConfig;
            TestParametersDefinition testParams;

            try
            {
                testConfig = testConfigJson.ToObject<TestConfigurationDefinition>();
            }
            catch (JsonSerializationException ex)
            {
                PublishContainerImages.WriteError($"No test will be run, invalid Json read in file {testConfigFiles.Single().FullName}, Json was: {testConfigJson}. Exception: {ex}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            var testParamsJson = _readJsonFileToDynamic(testParamsFiles.Single().FullName);

            try
            {
                testParams = testParamsJson.ToObject<TestParametersDefinition>();
            }
            catch (JsonSerializationException ex)
            {
                PublishContainerImages.WriteError($"No test will be run, invalid Json read in file {testParamsJson.Single().FullName}, Json was: {testParamsJson}. Exception: {ex}");
                return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(null, null);
            }

            return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(testConfig, testParams);
        }
    }
}

