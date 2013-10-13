namespace Wacton.Colonies.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Logic;

    public class WeightedColorsToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value[0] is Color && 
                value[1] is SolidColorBrush && value[2] is double &&
                value[3] is SolidColorBrush && value[4] is double &&
                value[5] is SolidColorBrush && value[6] is double &&
                value[7] is SolidColorBrush && value[8] is double)
            {
                var baseColor = (Color)value[0];
                var mineral = new WeightedColor(((SolidColorBrush)value[1]).Color, (double)value[2]);
                var damp = new WeightedColor(((SolidColorBrush)value[3]).Color, (double)value[4]);
                var heat = new WeightedColor(((SolidColorBrush)value[5]).Color, (double)value[6]);
                var poison = new WeightedColor(((SolidColorBrush)value[7]).Color, (double)value[8]);

                var environmentModifiers = new List<WeightedColor> { damp, heat, poison };
                return ColourLogic.EnvironmentBrush(baseColor, mineral, environmentModifiers);
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
