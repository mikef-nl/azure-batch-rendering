
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Autodesk.Max.MaxSDK.AssetManagement;
using Autodesk.Max.Plugins;

using BatchLabs.Max2016.Plugin.Max;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetHandler : AssetEnumCallback
    {
        private readonly Dictionary<string, string> _assets;

        public AssetHandler()
        {
            _assets = new Dictionary<string, string>();
        }

        public override void RecordAsset(IAssetUser asset)
        {
            if (_assets.ContainsKey(asset.IdAsString) == false)
            {
                _assets.Add(asset.IdAsString, asset.FullFilePath);
            }
        }

        public Dictionary<string, string> Assets => _assets;
    }

    public class AssetWrangler
    {
        public static async Task<Dictionary<string, string>> GetAssetDirs()
        {
            var assetHandler = new AssetHandler();

            try
            {
                Log.Instance.Debug("Calling EnumAuxFiles");
                await Task.Run(() => MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetHandler, 1 << 2 | 1 << 13));
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get assets from scene: {ex.Message}. {ex}");
            }

            Log.Instance.Debug("Returning");
            return assetHandler.Assets;
        }
    }
}
