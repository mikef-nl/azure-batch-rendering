using System;
using System.Collections.Generic;
using System.Text;

namespace PublishContainerImages
{
    public class ContainerImageDef
    {
        public string ContainerImage;

        public string PathToDockerFile;

        public string InstallerFileBlob;
    }
}
