
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BatchLabs.Max2016.Plugin.Max;
using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetWrangler
    {
        public static async Task<List<AssetFile>> GetFoundAssets()
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
    }
}
