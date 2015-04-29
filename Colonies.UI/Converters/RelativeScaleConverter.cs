namespace Wacton.Colonies.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class RelativeScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value[0] is double && value[1] is double)
            {
                var dimension = (double)value[0];
                var scale = (double)value[1];
                return dimension * scale;
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
