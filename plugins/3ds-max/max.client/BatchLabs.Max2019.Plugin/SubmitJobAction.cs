
using System;
using System.Windows;

#if !DEBUG
using System.Windows.Media.Imaging;
using System.Windows.Interop;
#endif

using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Max2019.Plugin.Max;
using BatchLabs.Plugin.Common.XAML;
using ManagedServices;

using MessageBox = System.Windows.Forms.MessageBox;

namespace BatchLabs.Max2019.Plugin
{
    public class SubmitJobAction : ActionBase
    {
        private MaxRequestInterceptor _maxRequestInterceptor;

        public override void InternalExecute()
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            coreInterface.PushPrompt("Gathering up information about the job");
            Log.Instance.Debug("Gathering up information about the job");

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
                    ShowInTaskbar = true
                };

                _maxRequestInterceptor = new MaxRequestInterceptor();
                var jobSubmissionForm = new JobSubmissionForm(LabsRequestHandler, _maxRequestInterceptor, Log.Instance);
                dialog.Content = jobSubmissionForm;

#if !DEBUG
                // only works in the context of 3ds Max
                var maxIcon = GetMaxIcon();
                if (maxIcon != null)
                {
                    dialog.Icon = maxIcon;
                }

                var windowHandle = new WindowInteropHelper(dialog);
                windowHandle.Owner = AppSDK.GetMaxHWND();
                AppSDK.ConfigureWindowForMax(dialog);
#endif
                dialog.ShowDialog();

            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{ex.Message}\n{ex}", "Error showing job submission form", true);
                MessageBox.Show($"Error showing job submission form.\n{ex.Message}\n{ex}");
            }
        }

#if !DEBUG
        private BitmapSource GetMaxIcon()
        {
            // only works in the context of 3ds Max
            try
            {
                var hBitmap = AppSDK.GetMainApplicationIcon().ToBitmap().GetHbitmap();
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{ex.Message}\n{ex}", "Error getting max icon");
                return null;
            }
        }
#endif

        public override string InternalActionText => "Submit a Job";
    }
}
