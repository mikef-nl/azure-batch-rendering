
using BatchLabs.Max2016.Plugin.Max;

namespace BatchLabs.Max2016.Plugin
{
    public class MonitorPoolsAction : ActionBase
    {
        public override void InternalExecute()
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Calling out to pools UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("pools");
        }

        public override string InternalActionText => "Monitor Your Pools";
    }
}
