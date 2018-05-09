
using System;
using System.Windows;

#if !DEBUG
using System.Windows.Media.Imaging;
using System.Windows.Interop;
#endif

using BatchLabs.Max2016.Plugin.Max;
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
                    Title = "Submit a Job to BatchLabs",
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.CanResizeWithGrip,
                    WindowStyle = WindowStyle.ToolWindow,
                    ShowInTaskbar = false
                };

                var jobSubmissionForm = new JobSubmissionForm(LabsRequestHandler);
                dialog.Content = jobSubmissionForm;

#if !DEBUG
                var maxIcon = GetMaxIcon();
                if (maxIcon != null)
                {
                    dialog.Icon = maxIcon;
                }

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

#if !DEBUG
        private BitmapSource GetMaxIcon()
        {
            try
            {
                var hBitmap = ManagedServices.AppSDK.GetMainApplicationIcon().ToBitmap().GetHbitmap();
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception)
            {
                // TODO: Figure out how to send logs to Max
                return null;
            }
        }
#endif

        public override string InternalActionText => "Submit a Job";
    }
}
