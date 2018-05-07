
using BatchLabs.Max2016.Plugin.Contract;

namespace BatchLabs.Max2016.Plugin
{
    public class ManageDataAction : ActionBase
    {
        public override void InternalExecute()
        {            
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Calling out to data UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("data");
        }

        public override string InternalActionText => "Monitor your Data";
    }
}
