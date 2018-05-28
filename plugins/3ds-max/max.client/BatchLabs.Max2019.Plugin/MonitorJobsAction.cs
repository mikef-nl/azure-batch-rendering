
using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Max2019.Plugin.Max;
using BatchLabs.Plugin.Common.Code;
using BatchLabs.Plugin.Common.Resources;

namespace BatchLabs.Max2019.Plugin
{
    public class MonitorJobsAction : ActionBase
    {
        public override void InternalExecute()
        {
            MaxGlobalInterface.Instance.COREInterface16.PushPrompt(Strings.MonitorJobs_Log);
            Log.Instance.Debug(Strings.MonitorJobs_Log);

            LabsRequestHandler.CallBatchLabs(Constants.BatchLabsUrs.Jobs);
        }

        public override string InternalActionText => Strings.MonitorJobs_ActionText;
    }
}
