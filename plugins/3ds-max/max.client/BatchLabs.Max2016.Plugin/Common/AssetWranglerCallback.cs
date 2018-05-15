
using System.Collections.Generic;

using Autodesk.Max.MaxSDK.AssetManagement;
using Autodesk.Max.Plugins;

using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetWranglerCallback : AssetEnumCallback
    {
        private readonly Dictionary<string, AssetFile> _assets;

        public AssetWranglerCallback()
        {
            _assets = new Dictionary<string, AssetFile>();
        }

        public override void RecordAsset(IAssetUser asset)
        {
            if (_assets.ContainsKey(asset.IdAsString) == false)
            {
                _assets.Add(asset.IdAsString, new AssetFile(asset));
            }
        }

        public Dictionary<string, AssetFile> AssetDictionary => _assets;

        public List<AssetFile> AssetList => new List<AssetFile>(_assets.Values);
    }
}
