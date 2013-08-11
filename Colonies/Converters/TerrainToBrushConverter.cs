namespace Wacton.Colonies.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

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
                var ratio = (double)value[1];

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

                terrainBrush = this.ApplyMineralBrush(terrainBrush, Brushes.Goldenrod, ratio);
                return terrainBrush;
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        private SolidColorBrush ApplyMineralBrush(SolidColorBrush baseBrush, SolidColorBrush mineralBrush, double mineralRatio)
        {
            var earthWithMineralColor = this.Interpolate(baseBrush.Color, mineralBrush.Color, mineralRatio);
            return new SolidColorBrush(earthWithMineralColor);
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
