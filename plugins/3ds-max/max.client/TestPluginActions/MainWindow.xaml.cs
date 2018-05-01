
using System.Windows;

using BatchLabsRendering;

namespace TestPluginActions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // NOTE: for these to work I need to inject a new GlobalInterface.Instance into the app somehow.

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
