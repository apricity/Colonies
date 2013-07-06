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

            if (Enum.IsDefined(typeof(Terrain), value))
            {
                var terrain = (Terrain)value;

                switch (terrain)
                {
                    case Terrain.Earth:
                        return Brushes.BurlyWood;
                    case Terrain.Grass:
                        return Brushes.ForestGreen;
                    case Terrain.Water:
                        return Brushes.DeepSkyBlue;
                    case Terrain.Fire:
                        return Brushes.Firebrick;
                    case Terrain.Unknown:
                        return Brushes.DimGray;
                    default:
                        return Brushes.Transparent;
                }
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
