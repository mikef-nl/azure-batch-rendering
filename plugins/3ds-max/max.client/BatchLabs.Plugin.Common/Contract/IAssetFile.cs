
using System;
using System.Windows.Input;

namespace BatchLabs.Plugin.Common.Contract
{
    public interface IAssetFile
    {
        event EventHandler RemoveFileInfo;

        string Id { get; set; }

        string FileName { get; set; }

        string FullFilePath { get; set; }

        bool IsSelected { get; set; }

        ICommand RemoveMissingFileCommand { get; }
    }
}
