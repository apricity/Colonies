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
                        return Brushes.Sienna;
                    case Terrain.Water:
                        return Brushes.CornflowerBlue;
                    case Terrain.Fire:
                        return Brushes.Tomato;
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
