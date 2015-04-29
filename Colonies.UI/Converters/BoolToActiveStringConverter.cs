namespace Wacton.Colonies.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BoolToActiveStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is bool)
            {
                var active = (bool)value;
                return active ? "Active" : "Inactive";
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
