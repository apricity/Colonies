namespace Colonies.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ActiveBoolToStringConverter : IValueConverter
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
                if (active)
                {
                    return "Active";
                }
                else
                {
                    return "Inactive";
                }
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
