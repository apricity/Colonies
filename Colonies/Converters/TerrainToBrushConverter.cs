namespace Wacton.Colonies.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;

    public class TerrainToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (Enum.IsDefined(typeof(Terrain), value[0]) && value[1] is double)
            {
                var terrain = (Terrain)value[0];
                var mineralRatio = (double)value[1];
                var dampRatio = (double)value[2];
                var heatRatio = (double)value[3];

                SolidColorBrush terrainBrush;
                switch (terrain)
                {
                    case Terrain.Earth:
                        terrainBrush = Brushes.Sienna;
                        break;
                    case Terrain.Water:
                        terrainBrush = Brushes.CornflowerBlue;
                        break;
                    case Terrain.Fire:
                        terrainBrush = Brushes.Tomato;
                        break;
                    default:
                        terrainBrush = Brushes.Transparent;
                        break;
                }

                // TODO: does the order of modification matter?
                terrainBrush = this.ModifyBrush(terrainBrush, Brushes.Goldenrod, mineralRatio);
                terrainBrush = this.ModifyBrush(terrainBrush, Brushes.CornflowerBlue, dampRatio);
                terrainBrush = this.ModifyBrush(terrainBrush, Brushes.Tomato, heatRatio);

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
