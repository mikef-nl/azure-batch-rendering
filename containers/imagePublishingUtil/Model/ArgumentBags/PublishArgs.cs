using System;
using System.Collections.Generic;
using System.Text;

namespace PublishContainerImages.Model.ArgumentBags
{
    class PublishArgs
    {
        public string TargetRelativeDirs;

        public string DockerInstallScriptsRootDir;

        public bool IncludeAntecendents;

        public bool IncludeDescendents;

        public string BuildVersion;

        public string ReleaseName;

        public string GitCommitSha;

        public string PublishRepoBase;

        public string PublishRepoUsername;

        public string PublishRepoPassword;

        public bool ShouldPushImages;
    }
}
