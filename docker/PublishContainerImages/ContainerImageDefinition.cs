using System;
using System.Collections.Generic;
using System.Text;

namespace PublishContainerImages
{
    public class ContainerImageDef
    {
        public ContainerImageDef()
        { }

        public string containerImage;

        public string pathToDockerFile;

        public string installerFileBlob;
    }
}
