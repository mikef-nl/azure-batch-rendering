
namespace BatchLabs.Max2016.Plugin.Models
{
    public class AssetFolder
    {
        public AssetFolder(string path, bool locked = false)
        {
            Path = path;
            CanRemove = !locked;
        }

        public string Path { get; set; }

        public bool CanRemove { get; set; }

        public bool IsSelected { get; set; }
    }
}
