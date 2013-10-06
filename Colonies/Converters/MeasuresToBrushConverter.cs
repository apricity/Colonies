namespace Wacton.Colonies.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

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

                // remove all zero-level brushes
                // order all element brushes by how strong their influence is
                var elements = new Dictionary<SolidColorBrush, double>
                                        {
                                            { Brushes.Tomato, heatRatio },
                                            { Brushes.CornflowerBlue, dampRatio }
                                        };

                var orderedElements = elements.Where(pair => Math.Abs(pair.Value - 0.0) > 0.0)
                                              .OrderByDescending(pair => pair.Value)
                                              .ToDictionary(pair => pair.Key, pair => pair.Value);

                // if there are no brushes to apply, return the current terrain brush
                if (orderedElements.Count == 0)
                {
                    return terrainBrush;
                }

                // if there is only one brush to apply, do so and return the result
                if (orderedElements.Count == 1)
                {
                    return this.ModifyBrush(terrainBrush, orderedElements.Single().Key, orderedElements.Single().Value);
                }

                // if there are multiple brushes to apply
                // calculate the blended element colour using their respective ratio
                // then apply the blended colour, using the highest ratio of the elements
                var blendedRatio = orderedElements.First().Value;
                var elementBrush = this.BlendBrushes(orderedElements);

                terrainBrush = this.ModifyBrush(terrainBrush, elementBrush, blendedRatio);
                return terrainBrush;
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        private SolidColorBrush BlendBrushes(Dictionary<SolidColorBrush, double> orderedElements)
        {
            var greaterRatio = orderedElements.First().Value;
            var elementBrush = orderedElements.First().Key;
            orderedElements.Remove(elementBrush);

            var hasCombinedAllElementBrushes = false;
            while (!hasCombinedAllElementBrushes)
            {
                var modifyBrush = orderedElements.First().Key;
                var lesserRatio = orderedElements.First().Value;

                var modifyRatio = lesserRatio / (greaterRatio + lesserRatio);
                elementBrush = this.ModifyBrush(elementBrush, modifyBrush, modifyRatio);
                orderedElements.Remove(modifyBrush);

                greaterRatio = lesserRatio;

                if (orderedElements.Count == 0)
                {
                    hasCombinedAllElementBrushes = true;
                }
            }

            return elementBrush;
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
