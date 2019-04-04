using System;
using System.Collections.Generic;
using System.Text;
using PublishContainerImages.Model;

namespace PublishContainerImages
{
    class ContainerImagePayload
    {
        public ContainerImageDefinition ContainerImageDefinition;

        public TestConfigurationDefinition TestConfigurationDefinition;

        public TestParametersDefinition TestParametersDefinition;
    }
}
