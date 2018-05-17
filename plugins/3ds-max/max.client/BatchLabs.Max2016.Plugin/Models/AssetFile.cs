
using System;
using System.Windows.Input;
using Autodesk.Max.MaxSDK.AssetManagement;
using BatchLabs.Max2016.Plugin.Commands;

namespace BatchLabs.Max2016.Plugin.Models
{
    public class AssetFile
    {
        public event EventHandler RemoveFileInfo;

        private RelayCommand _removeMissingFileCommand;

        public AssetFile(IAssetUser asset)
        {
            Id = asset.IdAsString;
            FullFilePath = asset.FullFilePath;
            FileName = asset.FileName;
        }

        public string Id { get; set; }

        public string FileName { get; set; }

        public string FullFilePath { get; set; }

        public bool IsSelected { get; set; }

        public ICommand RemoveMissingFileCommand => _removeMissingFileCommand ?? (_removeMissingFileCommand = new RelayCommand(OnRemoveItem));

        private void OnRemoveItem(object sender)
        {
            // send the item to the model to be removed
            RemoveFileInfo.Raise(sender, new EventArgs());
        }
    }
}
