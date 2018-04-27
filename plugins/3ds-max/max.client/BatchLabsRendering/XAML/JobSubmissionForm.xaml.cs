using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DrawingColor = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;

using Autodesk.Max;

namespace BatchLabsRendering.XAML
{
    /// <summary>
    /// Interaction logic for JobSubmissionForm.xaml
    /// </summary>
    public partial class JobSubmissionForm : UserControl
    {
        Window _parent;

        public JobSubmissionForm(Window parent)
        {
            _parent = parent;
            InitializeComponent();

            var global = GlobalInterface.Instance;
                        
            // Get current background color and match our dialog to it
            var bgColor = GetUiColorBrush(global.ColorManager, GuiColors.Background);
            LayoutRoot.Background = bgColor;


            var textColor = GetUiColorBrush(global.ColorManager, GuiColors.Text);
            LabelJobDetails.Foreground = textColor;
        }

        /// <summary>
        /// Get the brush color associated with the desired GuiColor and match our dialog to it.
        /// Due to a bug in the 3ds Max .NET API before 2017, you have to reverse the R and B values to assign color properly. 
        /// The Autodesk.Max assembly mapped them incorrectly.
        /// </summary>
        /// <param name="colorManager"></param>
        /// <param name="guiColor"></param>
        /// <returns></returns>
        private Brush GetUiColorBrush(IIColorManager colorManager, GuiColors guiColor)
        {
            DrawingColor color = colorManager.GetColor(guiColor);
            // 2017 and up: MediaColor mcolorText = MediaColor.FromRgb(dcolText.R, dcolText.G, dcolText.B);
            var mcolorText = MediaColor.FromRgb(color.B, color.G, color.R);

            return new SolidColorBrush(mcolorText);
        }
    }
}
