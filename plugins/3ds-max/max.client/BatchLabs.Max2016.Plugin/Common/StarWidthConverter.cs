
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class StarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listview = value as ListView;
            if (listview != null)
            {
                var width = listview.Width;
                Log.Instance.Debug($"############# List View Width: {width}");
                var gridView = listview.View as GridView;
                if (gridView != null)
                {
                    foreach (var column in gridView.Columns)
                    {
                        if (!Double.IsNaN(column.Width))
                        {
                            width -= column.Width;
                            Log.Instance.Debug($"############# Removing: {column.Width}");
                        }
                    }
                }

                // this is to take care of margin/padding
                Log.Instance.Debug($"############# WIDTH: {width}");
                return width - 5;
            }

            return 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
