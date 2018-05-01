
using BatchLabsRendering.Common;

namespace BatchLabsRendering
{
    public class MonitorJobsAction : ActionBase
    {
        public override void InternalExecute()
        {            
            var Interface = MaxGlobalInterface.Instance.COREInterface16;
            Interface.PushPrompt("Calling out to jobs UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("jobs");
        }

        public override string InternalActionText => "Monitor your Jobs";
    }
}
