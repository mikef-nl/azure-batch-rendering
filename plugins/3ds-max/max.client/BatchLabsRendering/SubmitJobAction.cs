using Autodesk.Max;

namespace BatchLabsRendering
{
    public class SubmitJobAction : ActionBase
    {
        public override void InternalExecute()
        {
            var Interface = GlobalInterface.Instance.COREInterface16;
            Interface.PushPrompt("Gathering up information about the job");
            OpenJobConfigWindow();
        }

        private void OpenJobConfigWindow()
        {

        }

        public override string InternalActionText => "Submit a Job";
    }
}
