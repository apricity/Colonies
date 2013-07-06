namespace Wacton.Colonies.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BoolToAliveOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is bool)
            {
                var isTrue = (bool)value;
                return isTrue ? 1 : 0.5;
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
