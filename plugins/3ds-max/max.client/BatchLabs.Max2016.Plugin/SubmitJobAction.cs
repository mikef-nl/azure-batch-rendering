
using System;
using System.Windows;
using System.Windows.Interop;

using BatchLabs.Max2016.Plugin.Contract;
using BatchLabs.Max2016.Plugin.XAML;

using MessageBox = System.Windows.Forms.MessageBox;

namespace BatchLabs.Max2016.Plugin
{
    public class SubmitJobAction : ActionBase
    {
        public override void InternalExecute()
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Gathering up information about the job");

            OpenJobConfigWindow();
        }

        private void OpenJobConfigWindow()
        {
            try
            {
                var dialog = new Window
                {
                    Title = "Submit me a job plz",
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize
                };

                var ctlExplode = new JobSubmissionForm(dialog);
                dialog.Content = ctlExplode;

#if (RELEASE)
                var windowHandle = new WindowInteropHelper(dialog);
                windowHandle.Owner = ManagedServices.AppSDK.GetMaxHWND();
                ManagedServices.AppSDK.ConfigureWindowForMax(dialog);
#endif
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
