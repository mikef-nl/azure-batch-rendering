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
        public static List<ContainerImagePayload> BuildPayloadFromDirectoryTree(DirectoryInfo root, TraversalMode mode, List<ContainerImagePayload> buildOrder)
        {
            FileInfo fileInfo = null;
            
            try
            {
                fileInfo = root.GetFiles(PublishContainerImages.ContainerImageDefinitionFilename).Single();
            }
            catch (UnauthorizedAccessException e)
            {
               PublishContainerImages.WriteLog(e.Message);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                PublishContainerImages.WriteLog(e.Message);
            }
            catch (InvalidOperationException e)
            {
                PublishContainerImages.WriteLog(e.Message);
            }

            if (fileInfo != null)
            {
                var containerImageDefinition = ReadContainerImageDefinition(fileInfo);

                var testConfigAndParams = TryReadTestConfigAndParams(fileInfo.Directory);

                buildOrder.Add(new ContainerImagePayload
                {
                    ContainerImageDefinition = containerImageDefinition,
                    TestConfigurationDefinition = testConfigAndParams.Item1,
                    TestParametersDefinition = testConfigAndParams.Item2,
                });

                if (mode == TraversalMode.Recursive)
                {
                    var subDirs = root.GetDirectories();

                    foreach (var dirInfo in subDirs)
                    {
                        BuildPayloadFromDirectoryTree(dirInfo, mode, buildOrder);
                    }
                }
            }

            return buildOrder;
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

        private static ContainerImageDefinition ReadContainerImageDefinition(FileInfo file)
        {
            ContainerImageDefinition containerImageDefinition;
            dynamic json = _readJsonFileToDynamic(file.FullName);
            try
            {
                containerImageDefinition = json.ToObject<ContainerImageDefinition>();
            }
            catch (JsonSerializationException ex)
            {
                PublishContainerImages.WriteError($"Invalid Json read in file {file}, Json was: {json}. Exception: {ex}");
                throw;
            }

            return containerImageDefinition;
        }

        private static Tuple<TestConfigurationDefinition, TestParametersDefinition> TryReadTestConfigAndParams(DirectoryInfo directory)
        {
            var testConfigFiles = directory.GetFiles(PublishContainerImages.TestConfigurationFilename);
            var testParamsFiles = directory.GetFiles(PublishContainerImages.TestParametersFilename);

            if (!testConfigFiles.Any())
            {
                PublishContainerImages.WriteLog($"No Test will be run, testConfiguration.json not found in directory {directory}");
                return null;
            }

            if (!testParamsFiles.Any())
            {
                PublishContainerImages.WriteLog($"No Test will be run, testParameters.json not found in directory {directory}");
                return null;
            }

            if (testConfigFiles.Length > 1)
            {
                PublishContainerImages.WriteLog($"No test will be run, more than one TestConfiguration.json file found in {directory}");
                return null;
            }

            if (testParamsFiles.Length > 1)
            {
                PublishContainerImages.WriteLog($"No test will be run, more than one TestParameters.json file found in {directory}");
                return null;
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
                return null;
            }

            var testParamsJson = _readJsonFileToDynamic(testParamsFiles.Single().FullName);

            try
            {
                testParams = testParamsJson.ToObject<TestParametersDefinition>();
            }
            catch (JsonSerializationException ex)
            {
                PublishContainerImages.WriteError($"No test will be run, invalid Json read in file {testParamsJson.Single().FullName}, Json was: {testParamsJson}. Exception: {ex}");
                return null;
            }

            return new Tuple<TestConfigurationDefinition, TestParametersDefinition>(testConfig, testParams);
        }
    }
}

