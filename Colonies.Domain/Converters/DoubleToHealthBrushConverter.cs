namespace Wacton.Colonies.Domain.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class DoubleToHealthLevelBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // assumes health level is between 0.0 - 1.0
            // 1.0 = 000, 255, 000
            // 0.5 = 255, 255, 000
            // 0.0 = 255, 000, 000
            if (value is double)
            {
                var health = (double)value;

                byte green = 255;
                byte red = 255;

                // account for red/green only applying to half of health by doubling
                if (health > 0.5)
                {
                    red = (byte)((1 - health) * 255 * 2);
                }
                else if (health < 0.5)
                {
                    green = (byte)(health * 255 * 2);
                }

                return new SolidColorBrush(Color.FromRgb(red, green, 0));
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
