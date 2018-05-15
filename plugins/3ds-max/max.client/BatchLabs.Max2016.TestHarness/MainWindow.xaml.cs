
using System.Windows;

using BatchLabs.Max2016.Plugin;
using BatchLabs.Max2016.Plugin.Max;
using BatchLabs.Max2016.Plugin.Contract.Stubs;

namespace BatchLabs.Max2016.TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var myInterface16 = (MaxGlobalInterface.Instance.COREInterface16 as Interface16Stub);
            if (myInterface16 != null)
            {
                myInterface16.PushMessage += (sender, message) => { ShowPrompt(message); };
            }
        }

        private void ShowPrompt(string prompt)
        {
            Footer.Content = prompt;
        }

        private void JobsButton_Click(object sender, RoutedEventArgs e)
        {
            var action = new MonitorJobsAction();
            action.InternalExecute();
        }

        private void PoolsButton_Click(object sender, RoutedEventArgs e)
        {
            var action = new MonitorPoolsAction();
            action.InternalExecute();
        }

        private void DataButton_Click(object sender, RoutedEventArgs e)
        {
            var action = new ManageDataAction();
            action.InternalExecute();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var action = new SubmitJobAction();
            action.InternalExecute();
        }
    }
}
