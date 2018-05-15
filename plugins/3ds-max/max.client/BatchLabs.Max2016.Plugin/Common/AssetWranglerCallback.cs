
using System.Collections.Generic;

using Autodesk.Max.MaxSDK.AssetManagement;
using Autodesk.Max.Plugins;

using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class AssetWranglerCallback : AssetEnumCallback
    {
        private readonly Dictionary<string, Asset> _assets;

        public AssetWranglerCallback()
        {
            _assets = new Dictionary<string, Asset>();
        }

        public override void RecordAsset(IAssetUser asset)
        {
            if (_assets.ContainsKey(asset.IdAsString) == false)
            {
                _assets.Add(asset.IdAsString, new Asset(asset));
            }
        }

        public Dictionary<string, Asset> AssetDictionary => _assets;

        public List<Asset> AssetList => new List<Asset>(_assets.Values);
    }
}
