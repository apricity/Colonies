namespace Colonies.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is Color)
            {
                var color = (Color)value;
                return new SolidColorBrush(color);
            }

            if (value is System.Drawing.Color)
            {
                var drawingColor = (System.Drawing.Color)value;
                var mediaColor = System.Windows.Media.Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B);
                return new SolidColorBrush(mediaColor);
            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
