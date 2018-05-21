﻿
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using BatchLabs.Max2016.Plugin.Commands;

namespace BatchLabs.Max2016.Plugin.Models
{
    public class AssetFolder : INotifyPropertyChanged
    {
        public event EventHandler RemoveDirectory;
        public event PropertyChangedEventHandler PropertyChanged;

        private string _path;
        private RelayCommand _removeDirectoryCommand;

        public AssetFolder(string path, bool locked = false)
        {
            _path = path;
            CanRemove = !locked;
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged();
            }
        }


        public bool CanRemove { get; set; }

        public bool IsSelected { get; set; }

        public ICommand RemoveDirectoryCommand => _removeDirectoryCommand ?? (_removeDirectoryCommand = new RelayCommand(RemoveItem, param => CanRemoveDirectory));

        public void RemoveItem(object sender)
        {
            // send the item to the model to be removed
            RemoveDirectory.Raise(sender, new EventArgs());
        }

        private bool CanRemoveDirectory => CanRemove;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
