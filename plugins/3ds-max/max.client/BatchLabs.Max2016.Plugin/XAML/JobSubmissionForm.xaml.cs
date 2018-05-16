
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
        private readonly string _maxFileFolderPath;
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


            _maxFileFolderPath = Path.GetDirectoryName(MaxGlobalInterface.Instance.COREInterface16.CurFilePath);
            SceneFile.Content = MaxGlobalInterface.Instance.COREInterface16.CurFileName;
            JobId.Text = Utils.ContainerizeMaxFile(SceneFile.Content.ToString());

            MissingAssets = new ObservableCollection<AssetFile>();
            AssetDirectories = new ObservableCollection<AssetFolder> { new AssetFolder(_maxFileFolderPath, true) };
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
            MissingLabel.Foreground = textColor;
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
                // get the assets in the project directory on disk
                Log.Instance.Debug($"maxFileFolder {_maxFileFolderPath}");
                var projectFolderFiles = FileActions.GetFileDictionaryWithLocations(_maxFileFolderPath);
                Log.Instance.Debug($"project dir files {projectFolderFiles.Keys.Count}");
                // TODO: Debug ... remove foreach log.
                foreach (var keyValue in projectFolderFiles)
                {
                    Log.Instance.Debug($"asset -> {keyValue.Key} --- paths -> {string.Join(":::", keyValue.Value)}");
                }

                // get the assets from the scene
                var sceneFiles = await AssetWrangler.GetFoundAssets();
                // TODO: Debug ... remove foreach log.
                Log.Instance.Debug($"got assets {sceneFiles.Count}");
                foreach (var asset in sceneFiles)
                {
                    var assetName = Path.GetFileName(asset.FileName) ?? "";
                    // Log.Instance.Debug($"checking asset -> {assetName} --- {asset.FileName} --- {asset.FullFilePath}");
                    if (projectFolderFiles.ContainsKey(assetName))
                    {
                        // found a file with the same name in the project directory, get the locations as it could exist in more than one folder on disk
                        var locations = projectFolderFiles[assetName];
                        if (string.IsNullOrEmpty(locations.Find(file => file == asset.FullFilePath)))
                        {
                            // the filename exists in the project direrctory, but not with the same path
                            // see if we can match up any directories and make a guess if its the correct one.
                            Log.Instance.Debug($"Did not find: {asset.FullFilePath}, locations: {locations.Count}, path separator: {Path.DirectorySeparatorChar}");
                            var matchingDirCount = FindMatchingDirectories(asset.FullFilePath, locations);
                            if (matchingDirCount == 0)
                            {
                                // add it to the missing asset list for the user to find
                                MissingAssets.Add(asset);
                                Log.Instance.Debug($"Add to missing asset list: {asset.FileName} :: {asset.FullFilePath}");
                            }
                            else
                            {
                                Log.Instance.Debug($"found partial match for [{asset.FileName}] in [{string.Join(":::", locations)}] with '{matchingDirCount}' matching dirs");
                            }
                        }
                        else
                        {
                            // found it and it mathces the path. do nothing.
                            Log.Instance.Debug($"Found in project dir {asset.FileName}");
                        }

                    }
                    else
                    {
                        // asset was not found in project directory. add to missing files.
                        Log.Instance.Debug($"Doesn't contain key: {assetName}");
                    }
                }



                // TODO: work this out from list of assets
                //AssetDirectories.Add(new AssetFolder(@"c:\program files (x86)\itoo software\forest pack pro\maps"));
                //AssetDirectories.Add(new AssetFolder(@"c:\program files (x86)\itoo software\forest pack pro\presets"));

                //var missing = await AssetWrangler.GetMissingAssets();
                //MissingAssets.AddRange(missing);



                //Log.Instance.Debug($"got missing assets {missing.Count}");
                //foreach (var asset in missing)
                //{
                //    Log.Instance.Debug($"missing -> {asset.FileName} --- {asset.FullFilePath}");
                //}
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get or display assets from scene: {ex.Message}. {ex}");
            }
        }

        /// <summary>
        /// Wander backwards up the directory tree looking for matches. This is for files that are in the project directory
        /// already, but 3ds Max is using them from another location, i.e. a pre-installed texture pack on the local machine.
        /// I.E. This file is marked as an asset by the max scene:
        ///     C:\program files (x86)\itoo software\forest pack pro\maps\presets\fp_grass_leaf_5.jpg
        /// 
        /// But it also exists in the project directory: 
        ///     D:\_azure\rendering\3dsmax\NOV\PipecatFX_FullMovie\itoo software\forest pack pro\maps\presets\fp_grass_leaf_5.jpg
        /// 
        /// If we find this is the case then we ignore the file in the program files folder as we don't need to upload 
        /// it again. 3ds Max will find it in the project directory, or in its propper location after the texture pack is 
        /// installed on the node.
        /// </summary>
        /// <param name="sceneFilePath"></param>
        /// <param name="diskLocations"></param>
        /// <returns></returns>
        private int FindMatchingDirectories(string sceneFilePath, List<string> diskLocations)
        {
            var locationMatch = new Dictionary<string, int>();
            diskLocations.ForEach(location =>
            {
                locationMatch[location] = 0;
                var sceneFileDirectory = FileActions.GetFileInfo(sceneFilePath).Directory;
                var diskLocationDirectory = FileActions.GetFileInfo(location).Directory;
                while (sceneFileDirectory != null && diskLocationDirectory != null)
                {
                    if (sceneFileDirectory.Name.Equals(diskLocationDirectory.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        locationMatch[location]++;
                    }
                    else
                    {
                        // the moment we don't match, break out of loop.
                        break;
                    }

                    // move to the parent directory.
                    sceneFileDirectory = sceneFileDirectory.Parent;
                    diskLocationDirectory = diskLocationDirectory.Parent;
                }
            });
            
            return locationMatch.Values.Max();
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
