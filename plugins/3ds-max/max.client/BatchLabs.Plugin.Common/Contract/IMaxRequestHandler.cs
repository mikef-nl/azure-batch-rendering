
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using BatchLabs.Plugin.Common.Code;

namespace BatchLabs.Plugin.Common.Contract
{
    public interface IMaxRequestHandler
    {
        /// <summary>
        /// Gets the current scene file name including the full directory path
        /// </summary>
        string CurrentSceneFilePath { get; }

        /// <summary>
        /// Gets the current scene file name without the path
        /// </summary>
        string CurrentSceneFileName { get; }

        /// <summary>
        /// Gets the render width of the scene in pixels
        /// </summary>
        int RenderWidth { get; }

        /// <summary>
        /// Gets the render height of the scene in pixels
        /// </summary>
        int RenderHeight { get; }

        /// <summary>
        /// Gets the first frame number in the scene
        /// </summary>
        int FrameStart { get; }

        /// <summary>
        /// Get the brush color that Max uses to paint the application background
        /// and match our dialog style colors to it.
        /// </summary>
        Brush GetUiColorBrush(BrushColorEnum color);

        /// <summary>
        /// Get all UI colors for testing.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<string, Brush>> GetAllColorBrushes();

        /// <summary>
        /// Parse the scene for rendering assets. Note that this is pretty slow for large scenes. 
        /// Need to figure out how to use the AssetMananger as the scene files are already parsed on 
        /// startup.
        /// </summary>
        /// <returns></returns>
        Task<List<IAssetFile>> GetFoundSceneAssets();

        /// <summary>
        /// Parse the scene for missing assets
        /// </summary>
        /// <returns></returns>
        Task<List<IAssetFile>> GetMissingAssets();
    }
}
