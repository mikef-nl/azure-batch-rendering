
using BatchLabs.Max2016.Plugin.Common;
using BatchLabs.Max2016.Plugin.Max;

namespace BatchLabs.Max2016.Plugin
{
    public class MonitorJobsAction : ActionBase
    {
        public override void InternalExecute()
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Calling out to jobs UI in BatchLabs");
            Log.Instance.Debug("Calling out to jobs UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("jobs");
        }

        public override string InternalActionText => "Monitor Your Jobs";
    }
}
