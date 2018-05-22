
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

using Autodesk.Max;
using Autodesk.Max.IColorManager;
using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Plugin.Common.Contract;

using MediaColor = System.Windows.Media.Color;

namespace BatchLabs.Max2019.Plugin.Max
{
    public class MaxRequestInterceptor : IMaxRequestHandler
    {
        public string CurrentSceneFilePath => MaxGlobalInterface.Instance.COREInterface16.CurFilePath;

        public string CurrentSceneFileName => MaxGlobalInterface.Instance.COREInterface16.CurFileName;

        public int RenderWidth => MaxGlobalInterface.Instance.COREInterface16.RendWidth;

        public int RenderHeight => MaxGlobalInterface.Instance.COREInterface16.RendHeight;

        public Brush GetUiColorBrush()
        {
            return GetUiColorBrush(MaxGlobalInterface.Instance.ColorManager, GuiColors.Background);
        }

        /// <summary>
        /// get the brish for 
        /// </summary>
        /// <returns></returns>
        public Brush GetTextColorBrush()
        {
            return GetUiColorBrush(MaxGlobalInterface.Instance.ColorManager, GuiColors.Text);
        }

        public async Task<List<IAssetFile>> GetFoundSceneAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumAll);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get assets from scene: {ex.Message}. {ex}");
            }

            return new List<IAssetFile>();
        }

        public async Task<List<IAssetFile>> GetMissingAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetFlags.FileEnumMissingActive);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to get missing assets from scene: {ex.Message}. {ex}");
            }


            return new List<IAssetFile>();
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
            var color = colorManager.GetColor(guiColor, State.Normal);
            var mcolorText = MediaColor.FromRgb(color.R, color.G, color.B);

            return new SolidColorBrush(mcolorText);
        }
    }
}
