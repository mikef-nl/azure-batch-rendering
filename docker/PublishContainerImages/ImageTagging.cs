using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace PublishContainerImages
{
    static class ImageTagging
    {
        public static string _fetchImageTag(string blobMd5)
        {
            var gitCommitSha = _getGitHeadShortSha();

            var sanitizedblobMd5 = _sanitizeBase64StringForDockerTag(blobMd5);

            var versionTag = $"git-{gitCommitSha}-blob-{sanitizedblobMd5}";

            return versionTag;
        }

        private static string _sanitizeBase64StringForDockerTag(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return base64;
            }

            return base64.Replace('/', '_').Replace('+', '.').TrimEnd('=').Substring(0, 7);
        }

        private static string _getGitHeadShortSha()
        {
            var repoPath = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\..\\..\\.git";

            using (var repo = new Repository(repoPath))
            {
                var sha = repo.Commits.First().Sha;
                return sha.Substring(0, 7);
            }
        }
    }
}
