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

            if (value[0] is Color && value[1] is WeightedColor && value[2] is WeightedColor && value[3] is WeightedColor && value[4] is WeightedColor)
            {
                var baseColor = (Color)value[0];
                var mineral = (WeightedColor)value[1];
                var damp = (WeightedColor)value[2];
                var heat = (WeightedColor)value[3];
                var poison = (WeightedColor)value[4];

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
