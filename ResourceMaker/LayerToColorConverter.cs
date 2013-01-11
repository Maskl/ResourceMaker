using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ResourceMaker
{
    public class LayerToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var layerName = (string)value;
            var forBorder = Boolean.Parse((string)parameter);

            var rand = new Random(layerName.GetHashCode());
            var x = rand.Next(3);
            var color = Color.FromArgb((byte)(forBorder ? 255 : 64),
                                       (byte)((x == 0) ? 223 : rand.Next(63, 127)),
                                       (byte)((x == 2) ? 223 : rand.Next(63, 127)),
                                       (byte)((x == 1) ? 223 : rand.Next(63, 127)));

            return color.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}