
using Autodesk.Max.MaxSDK.AssetManagement;

namespace BatchLabs.Max2016.Plugin.Models
{
    public class AssetFile
    {
        public AssetFile(IAssetUser asset)
        {
            Id = asset.IdAsString;
            FullFilePath = asset.FullFilePath;
            FileName = asset.FileName;
        }

        public string Id { get; set; }

        public string FileName { get; set; }

        public string FullFilePath { get; set; }
    }
}
