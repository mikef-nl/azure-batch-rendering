
using BatchLabsRendering.Common;

namespace BatchLabsRendering
{
    public class MonitorPoolsAction : ActionBase
    {
        public override void InternalExecute()
        {
            var Interface = MaxGlobalInterface.Instance.COREInterface16;
            Interface.PushPrompt("Calling out to pools UI in BatchLabs");

            LabsRequestHandler.CallBatchLabs("pools");
        }

        public override string InternalActionText => "Monitor your Pools";
    }
}
