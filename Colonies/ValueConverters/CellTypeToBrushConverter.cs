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

    public class CellTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cellType = (CellType)value;
            System.Drawing.Color drawingColor;

            switch (cellType)
            {
                case CellType.Earth:
                    drawingColor = System.Drawing.Color.BurlyWood;
                    break;
                case CellType.Grass:
                    drawingColor = System.Drawing.Color.ForestGreen;
                    break;
                case CellType.Water:
                    drawingColor = System.Drawing.Color.DeepSkyBlue;
                    break;
                case CellType.Fire:
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
