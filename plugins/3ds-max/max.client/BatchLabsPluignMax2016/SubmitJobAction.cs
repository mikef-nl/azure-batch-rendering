
using System;
using System.Windows;
using System.Windows.Interop;

using BatchLabsRendering.Common;
using BatchLabsRendering.XAML;

using MessageBox = System.Windows.Forms.MessageBox;

namespace BatchLabsRendering
{
    public class SubmitJobAction : ActionBase
    {
        public override void InternalExecute()
        {
            var Interface = MaxGlobalInterface.Instance.COREInterface16;
            Interface.PushPrompt("Gathering up information about the job");
            OpenJobConfigWindow();
        }

        private void OpenJobConfigWindow()
        {
            try
            {
                Window dialog = new Window();
                dialog.Title = "Submit me a job plz";
                dialog.SizeToContent = SizeToContent.WidthAndHeight;

                JobSubmissionForm ctlExplode = new JobSubmissionForm(dialog);
                dialog.Content = ctlExplode;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.ShowInTaskbar = false;
                dialog.ResizeMode = ResizeMode.NoResize;

                WindowInteropHelper windowHandle = new WindowInteropHelper(dialog);
                windowHandle.Owner = ManagedServices.AppSDK.GetMaxHWND();
                ManagedServices.AppSDK.ConfigureWindowForMax(dialog);

                // modal version; this prevents changes being made to model while our dialog is running, etc.
                dialog.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing job submission form.\n{ex.Message}\n{ex}");
            }
        }

        public override string InternalActionText => "Submit a Job";
    }
}
