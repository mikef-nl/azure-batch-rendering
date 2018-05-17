
using System;
using System.Windows.Input;

using BatchLabs.Max2016.Plugin.Commands;

namespace BatchLabs.Max2016.Plugin.Models
{
    public class AssetFolder
    {
        public event EventHandler RemoveDirectory;

        private RelayCommand _removeDirectoryCommand;

        public AssetFolder(string path, bool locked = false)
        {
            Path = path;
            CanRemove = !locked;
        }

        public string Path { get; set; }

        public bool CanRemove { get; set; }

        public bool IsSelected { get; set; }

        public ICommand RemoveDirectoryCommand => _removeDirectoryCommand ?? (_removeDirectoryCommand = new RelayCommand(OnRemoveItem, param => CanRemoveDirectory));

        private void OnRemoveItem(object sender)
        {
            // send the item to the model to be removed
            RemoveDirectory.Raise(sender, new EventArgs());
        }

        private bool CanRemoveDirectory => CanRemove;
    }
}
