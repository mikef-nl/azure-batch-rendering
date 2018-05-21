
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BatchLabs.Max2016.Plugin.Max;
using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetWrangler
    {
        public static async Task<List<AssetFile>> GetFoundSceneAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumAll);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get assets from scene: {ex.Message}. {ex}");
            }

            return new List<AssetFile>();
        }

        public static async Task<List<AssetFile>> GetMissingAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumMissingActive);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get missing assets from scene: {ex.Message}. {ex}");
            }

            
            return new List<AssetFile>();
        }

        /// <summary>
        /// Get a list of all files that exist in the current project directory.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetProjectFiles(string projectFolder)
        {
            var projectFolderFiles = FileActions.GetFileDictionaryWithLocations(projectFolder);
            Log.Instance.Debug($"found '{projectFolderFiles.Keys.Count}' files in the project directory: '{projectFolder}'");

            return projectFolderFiles;
        }

        /// <summary>
        /// Wander backwards up the directory tree looking for matches. This is for files that are in the project directory
        /// already, but 3ds Max is using them from another location, i.e. a pre-installed texture pack on the local machine.
        /// I.E. This file is marked as an asset by the max scene:
        ///     C:\program files (x86)\itoo software\forest pack pro\maps\presets\fp_grass_leaf_5.jpg
        /// 
        /// But it also exists in the project directory: 
        ///     D:\_azure\rendering\3dsmax\NOV\PipecatFX_FullMovie\itoo software\forest pack pro\maps\presets\fp_grass_leaf_5.jpg
        /// 
        /// If we find this is the case then we ignore the file in the program files folder as we don't need to upload 
        /// it again. 3ds Max will find it in the project directory, or in its propper location after the texture pack is 
        /// installed on the node.
        /// </summary>
        /// <param name="sceneFilePath"></param>
        /// <param name="diskLocations"></param>
        /// <returns></returns>
        public static int FindMatchingDirectories(string sceneFilePath, List<string> diskLocations)
        {
            var locationMatch = new Dictionary<string, int>();
            diskLocations.ForEach(location =>
            {
                locationMatch[location] = 0;
                var sceneFileDirectory = FileActions.GetFileInfo(sceneFilePath).Directory;
                var diskLocationDirectory = FileActions.GetFileInfo(location).Directory;
                while (sceneFileDirectory != null && diskLocationDirectory != null)
                {
                    if (sceneFileDirectory.Name.Equals(diskLocationDirectory.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        locationMatch[location]++;
                    }
                    else
                    {
                        // the moment we don't match, break out of loop.
                        break;
                    }

                    // move to the parent directory.
                    sceneFileDirectory = sceneFileDirectory.Parent;
                    diskLocationDirectory = diskLocationDirectory.Parent;
                }
            });

            return locationMatch.Values.Max();
        }
    }
}
