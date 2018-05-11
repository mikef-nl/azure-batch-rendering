
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Autodesk.Max;
using BatchLabs.Max2016.Plugin.Common;
using BatchLabs.Max2016.Plugin.Max;

using MediaColor = System.Windows.Media.Color;

namespace BatchLabs.Max2016.Plugin.XAML
{
    /// <summary>
    /// Window for populating job data before it's sent to BatchLabs for processing.
    /// </summary>
    public partial class JobSubmissionForm
    {
        private const int MaxFileGroupLength = 55;
        private const string Prefix = "3dsmax-";

        private static readonly Regex UnderscoresAndMultipleDashes = new Regex("[_-]+");
        private static readonly char[] ForbiddenLeadingTrailingContainerNameChars = { '-' };

        private readonly BatchLabsRequestHandler _labsRequestHandler;
        private readonly int _maxUsableLength = MaxFileGroupLength - Prefix.Length;

        public JobSubmissionForm(BatchLabsRequestHandler labsRequestHandler)
        {
            _labsRequestHandler = labsRequestHandler;

            InitializeComponent();
            DataContext = this;

            SetWindowColor();
            SetLabelColors();

            Templates = TemplateHelper.GetApplicationTemplates();
            SelectedTemplate = "standard";

            Renderers = TemplateHelper.GetRenderers();
            SelectedRenderer = "vray";
            
            SceneFile.Content = MaxGlobalInterface.Instance.COREInterface16.CurFileName;
            JobId.Text = ContainerizeMaxFile(SceneFile.Content.ToString());
        }

        public string SelectedRenderer { get; set; }

        public List<KeyValuePair<string, string>> Renderers { get; }

        public string SelectedTemplate { get; set; }

        public List<KeyValuePair<string, string>> Templates { get; }

        /// <summary>
        /// Get current 3ds Max background color and match our dialog to it
        /// </summary>
        private void SetWindowColor()
        {
            LayoutRoot.Background = GetUiColorBrush(MaxGlobalInterface.Instance.ColorManager, GuiColors.Background);
        }

        /// <summary>
        /// Get current 3ds Max text color and match our labels to it
        /// </summary>
        private void SetLabelColors()
        {
            var textColor = GetUiColorBrush(MaxGlobalInterface.Instance.ColorManager, GuiColors.Text);
            JobDetailsTitle.Foreground = textColor;
            JobNameLabel.Foreground = textColor;
            TemplateLabel.Foreground = textColor;
            FrameStartLabel.Foreground = textColor;
            FrameEndLabel.Foreground = textColor;
            RendererLabel.Foreground = textColor;
            AssetsTitle.Foreground = textColor;
            SceneFileLabel.Foreground = textColor;
            SceneFile.Foreground = textColor;
            AssetsLabel.Foreground = textColor;
        }

        /// <summary>
        /// Get the brush color associated with the desired GuiColor and match our
        /// dialog style colors to it.
        /// </summary>
        /// <param name="colorManager"></param>
        /// <param name="guiColor"></param>
        /// <returns></returns>
        private static Brush GetUiColorBrush(IIColorManager colorManager, GuiColors guiColor)
        {
            var color = colorManager.GetColor(guiColor);
            var mcolorText = MediaColor.FromRgb(color.R, color.G, color.B);

            return new SolidColorBrush(mcolorText);
        }

        /// <summary>
        /// Input validator for the frameStart and frameEnd textboxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidateNumber(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Sanitize the 3ds max file name into a valid storage container name so we can
        /// use it for a file group name should we choose to.
        /// </summary>
        /// <param name="maxFile"></param>
        /// <returns></returns>
        private string ContainerizeMaxFile(string maxFile)
        {
            if (string.IsNullOrEmpty(maxFile))
            {
                return string.Empty;
            }

            // get the filename only and lower case it
            var sansExtension = Path.GetFileNameWithoutExtension(maxFile).ToLower();
            
            // replace underscores and multiple dashes
            sansExtension = UnderscoresAndMultipleDashes.Replace(sansExtension, "-");

            // check that the filename is not too long, if it is then trim it
            if (sansExtension.Length > _maxUsableLength)
            {
                sansExtension = sansExtension.Substring(0, _maxUsableLength);
            }

            // return after replacing any start and end hyphens
            return sansExtension.Trim(ForbiddenLeadingTrailingContainerNameChars);
        }

        /// <summary>
        /// Handle submit button click. Set up any values we want to pass to BatchLabs and 
        /// then call the request handler to make the call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var coreInterface = MaxGlobalInterface.Instance.COREInterface16;
            var launchUrl = $"market/3dsmax/actions/{SelectedTemplate}/submit";

            // set up the basic arguments
            var arguments = new Dictionary<string, string>
            {
                ["auto-pool"] = "0",
                ["input-parameter"] = "inputFilegroup",
                ["jobName"] = JobId.Text,
                ["frameStart"] = FrameStart.Text,
                ["frameEnd"] = FrameEnd.Text,
                ["renderer"] = SelectedRenderer
            };

            // if we have a max file loaded then we pass this as well
            if (false == string.IsNullOrEmpty(coreInterface.CurFileName))
            {
                arguments["sceneFile"] = coreInterface.CurFileName;
                arguments["asset-container"] = ContainerizeMaxFile(coreInterface.CurFileName);
                arguments["asset-paths"] = coreInterface.CurFilePath;
            }

#if DEBUG
            // show what we are about to send to labs
            var debug = $"Launch:{launchUrl}\n";
            foreach (var arg in arguments)
            {
                debug = string.Concat(debug, $"{arg.Key}:{arg.Value}\n");
            }

            Log.Instance.Debug(debug);
#endif

            // call labs with all the parameters, will open correct gallery template
            _labsRequestHandler.CallBatchLabs(launchUrl, arguments);
        }
    }
}
