using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Drawing;

    public class TerrainToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
