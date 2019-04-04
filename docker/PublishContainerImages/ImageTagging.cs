using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PublishContainerImages
{
    static class ImageTagging
    {
        public static string _fetchImageTag(string blobMd5, string gitSha)
        {
            var gitShortSha =  gitSha.Substring(0, 7);

            var sanitizedblobMd5 = _sanitizeBase64StringForDockerTag(blobMd5);

            var versionTag = $"git-{gitShortSha}-blob-{sanitizedblobMd5}";

            return versionTag;
        }

        private static string _sanitizeBase64StringForDockerTag(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return "NONE";  //default if no installer blob is used
            }

            return base64.Replace('/', '_').Replace('+', '.').TrimEnd('=').Substring(0, 7);
        }
    }
}
