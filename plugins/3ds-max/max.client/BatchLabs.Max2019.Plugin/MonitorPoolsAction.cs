
using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Max2019.Plugin.Max;

namespace BatchLabs.Max2019.Plugin
{
    public class MonitorPoolsAction : ActionBase
    {
        public override void InternalExecute()
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Calling out to pools UI in BatchLabs");
            Log.Instance.Debug("Calling out to pools UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("pools");
        }

        public override string InternalActionText => "Monitor Your Pools";
    }
}
