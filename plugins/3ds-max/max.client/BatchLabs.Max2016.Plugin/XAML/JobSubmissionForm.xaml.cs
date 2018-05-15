
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Autodesk.Max;

using BatchLabs.Max2016.Plugin.Common;
using BatchLabs.Max2016.Plugin.Max;
using BatchLabs.Max2016.Plugin.Models;

using MediaColor = System.Windows.Media.Color;

namespace BatchLabs.Max2016.Plugin.XAML
{
    /// <summary>
    /// Window for populating job data before it's sent to BatchLabs for processing.
    /// </summary>
    public partial class JobSubmissionForm
    {
        private readonly BatchLabsRequestHandler _labsRequestHandler;
        
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
            JobId.Text = Utils.ContainerizeMaxFile(SceneFile.Content.ToString());

            MissingAssets = new ObservableCollection<AssetFile>();
            AssetDirectories = new ObservableCollection<AssetFolder>();
            SetAssetCollection();
        }

        public string SelectedRenderer { get; set; }

        public List<KeyValuePair<string, string>> Renderers { get; }

        public string SelectedTemplate { get; set; }

        public List<KeyValuePair<string, string>> Templates { get; }

        public ObservableCollection<AssetFolder> AssetDirectories { get; set; }

        public ObservableCollection<AssetFile> MissingAssets { get; set; }

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

        private async void SetAssetCollection()
        {
            try
            {
                var assets = await AssetWrangler.GetFoundAssets();
                var maxFileFolder = Path.GetDirectoryName(MaxGlobalInterface.Instance.COREInterface16.CurFilePath);
                AssetDirectories.Add(new AssetFolder(maxFileFolder, true));

                // TODO: work this out from list of assets
                AssetDirectories.Add(new AssetFolder(@"c:\program files (x86)\itoo software\forest pack pro\maps"));
                AssetDirectories.Add(new AssetFolder(@"c:\program files (x86)\itoo software\forest pack pro\presets"));

                var missing = await AssetWrangler.GetMissingAssets();
                MissingAssets.AddRange(missing);

                // TODO: Debug ... remove below here.
                Log.Instance.Debug($"got assets {assets.Count}");
                foreach (var asset in assets)
                {
                    Log.Instance.Debug($"asset -> {asset.FileName} --- {asset.FullFilePath}");
                }

                Log.Instance.Debug($"got missing assets {missing.Count}");
                foreach (var asset in missing)
                {
                    Log.Instance.Debug($"missing -> {asset.FileName} --- {asset.FullFilePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get or display assets from scene: {ex.Message}. {ex}");
            }
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
                arguments["asset-container"] = Utils.ContainerizeMaxFile(coreInterface.CurFileName);
                arguments["asset-paths"] = string.Join(",", from folder in AssetDirectories select folder.Path);
                
            }

#if DEBUG
            // show what we are about to send to labs
            var debug = $"Launch:{launchUrl}\n";
            foreach (var arg in arguments)
            {
                debug = string.Concat(debug, $"{arg.Key}:{Uri.EscapeDataString(arg.Value)}\n");
            }

            Log.Instance.Debug(debug);
#endif

            // call labs with all the parameters, will open correct gallery template
            _labsRequestHandler.CallBatchLabs(launchUrl, arguments);
        }
    }
}
