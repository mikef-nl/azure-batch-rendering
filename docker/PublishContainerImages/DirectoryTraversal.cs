using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PublishContainerImages
{
    public static class DirectoryTraversal
    {
        public static List<ContainerImageDef> ImageBuildOrderFromDirectoryTree(System.IO.DirectoryInfo root, List<ContainerImageDef> buildOrder)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("containerImage.json");
            }
            catch (UnauthorizedAccessException e)
            {
               PublishContainerImages.WriteLog(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    dynamic json = _readContainerImagesJson(fi.FullName);
                    try
                    {
                        buildOrder.Add(json.ToObject<ContainerImageDef>());
                    }
                    catch (JsonSerializationException ex)
                    {
                        PublishContainerImages.WriteError($"Invalid Json read in file {fi}, Json was: {json}. Exception: {ex}");
                    }
                    
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    ImageBuildOrderFromDirectoryTree(dirInfo, buildOrder);
                }
            }

            return buildOrder;
        }

        private static dynamic _readContainerImagesJson(string filePath)
        {
            string jsonStr;
            using (var streamReader = new StreamReader(filePath))
            {
                jsonStr = streamReader.ReadToEnd();
            }

            dynamic json = JsonConvert.DeserializeObject(jsonStr);

            return json;
        }
    }
}

