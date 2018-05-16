
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BatchLabs.Max2016.Plugin.Common
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> GetAllDirectoryFiles(this DirectoryInfo dirInfo, bool excludeHidden)
        {
            var result = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
            return excludeHidden
                ? result.Where(file => (file.Attributes & FileAttributes.Hidden) == 0)
                : result;
        }
    }
}
