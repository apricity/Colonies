namespace Wacton.Colonies.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;

    public class MeasuresToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value[0] is double && value[1] is double && value[2] is double)
            {
                var mineralRatio = (double)value[0];
                var dampRatio = (double)value[1];
                var heatRatio = (double)value[2];

                // set up the standard default brush
                // and modify it by how much mineral is available
                var terrainBrush = Brushes.Tan;
                terrainBrush = this.ModifyBrush(terrainBrush, Brushes.Goldenrod, mineralRatio);

                // order all element brushes by how strong their influence is
                // apply the strongest first, and overlay the weaker ones afterwards
                var elementBrushes = new Dictionary<SolidColorBrush, double>
                                        {
                                            { Brushes.Tomato, heatRatio },
                                            { Brushes.CornflowerBlue, dampRatio }
                                        };

                var orderedBrushes = elementBrushes.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                terrainBrush = orderedBrushes.Aggregate(terrainBrush, (current, orderedBrush) => this.ModifyBrush(current, orderedBrush.Key, orderedBrush.Value));
                return terrainBrush;
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        private SolidColorBrush ModifyBrush(SolidColorBrush baseBrush, SolidColorBrush modifyBrush, double modifyRatio)
        {
            var modifiedColor = this.Interpolate(baseBrush.Color, modifyBrush.Color, modifyRatio);
            return new SolidColorBrush(modifiedColor);
        }

        private Color Interpolate(Color baseColor, Color targetColor, double targetRatio)
        {
            if (targetRatio < 0 || targetRatio > 1)
            {
                throw new ArgumentOutOfRangeException("targetRatio");
            }

            var alphaDifference = targetColor.A - baseColor.A;
            var redDifference = targetColor.R - baseColor.R;
            var greenDifference = targetColor.G - baseColor.G;
            var blueDifference = targetColor.B - baseColor.B;

            return new Color
                       {
                           A = (byte)(baseColor.A + (alphaDifference * targetRatio)),
                           R = (byte)(baseColor.R + (redDifference * targetRatio)),
                           G = (byte)(baseColor.G + (greenDifference * targetRatio)),
                           B = (byte)(baseColor.B + (blueDifference * targetRatio))
                       };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
