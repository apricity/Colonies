namespace Wacton.Colonies.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using Wacton.Colonies.Logic;

    public class MeasuresToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value[0] is double && value[1] is double && value[2] is double && value[3] is double)
            {
                var mineralRatio = (double)value[0];
                var dampRatio = (double)value[1];
                var heatRatio = (double)value[2];
                var poisonRatio = (double)value[3];

                // set up the standard default brush
                // and modify it by how much mineral is available
                var terrainBrush = Brushes.Tan;
                terrainBrush = this.ModifyBrush(terrainBrush, Brushes.Goldenrod, mineralRatio);

                // remove all zero-level brushes
                // order all element brushes by how strong their influence is
                var elements = new Dictionary<SolidColorBrush, double>
                                        {
                                            { Brushes.Tomato, heatRatio },
                                            { Brushes.CornflowerBlue, dampRatio },
                                            { Brushes.MediumAquamarine, poisonRatio }
                                        };

                return ColourLogic.TerrainBrush(elements, terrainBrush);
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        private SolidColorBrush ModifyBrush(SolidColorBrush baseBrush, SolidColorBrush modifyBrush, double modifyRatio)
        {
            var modifiedColor = this.Interpolate(baseBrush.Color, modifyBrush.Color, modifyRatio);
            return new SolidColorBrush(modifiedColor);
        }

        private Color Interpolate(Color baseColor, Color modifyColor, double modifyRatio)
        {
            if (modifyRatio < 0 || modifyRatio > 1)
            {
                throw new ArgumentOutOfRangeException("modifyRatio");
            }

            var alphaDifference = modifyColor.A - baseColor.A;
            var redDifference = modifyColor.R - baseColor.R;
            var greenDifference = modifyColor.G - baseColor.G;
            var blueDifference = modifyColor.B - baseColor.B;

            return new Color
                       {
                           A = (byte)(baseColor.A + (alphaDifference * modifyRatio)),
                           R = (byte)(baseColor.R + (redDifference * modifyRatio)),
                           G = (byte)(baseColor.G + (greenDifference * modifyRatio)),
                           B = (byte)(baseColor.B + (blueDifference * modifyRatio))
                       };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
