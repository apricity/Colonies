namespace Wacton.Colonies.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class TerrainToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is Terrain)
            {
                var terrain = (Terrain)value;
                System.Drawing.Color drawingColor;

                switch (terrain)
                {
                    case Terrain.Earth:
                        drawingColor = System.Drawing.Color.BurlyWood;
                        break;
                    case Terrain.Grass:
                        drawingColor = System.Drawing.Color.ForestGreen;
                        break;
                    case Terrain.Water:
                        drawingColor = System.Drawing.Color.DeepSkyBlue;
                        break;
                    case Terrain.Fire:
                        drawingColor = System.Drawing.Color.Firebrick;
                        break;
                    default:
                        drawingColor = System.Drawing.Color.Gray;
                        break;
                }

                var mediaColor = System.Windows.Media.Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B);
                return new SolidColorBrush(mediaColor);
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
