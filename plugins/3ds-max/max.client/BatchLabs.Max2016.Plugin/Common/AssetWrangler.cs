
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BatchLabs.Max2016.Plugin.Max;
using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetWrangler
    {
        public static async Task<List<Asset>> GetFoundAssets()
        {
            try
            {
                Log.Instance.Debug("Calling EnumAuxFiles");
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() => MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumAll));

                Log.Instance.Debug("Returning");
                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get assets from scene: {ex.Message}. {ex}");
            }

            return new List<Asset>();
        }

        public static async Task<List<Asset>> GetMissingAssets()
        {
            try
            {
                Log.Instance.Debug("Calling EnumAuxFiles");
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() => MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumMissingActive));

                Log.Instance.Debug("Returning");
                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get missing assets from scene: {ex.Message}. {ex}");
            }

            
            return new List<Asset>();
        }
    }
}
