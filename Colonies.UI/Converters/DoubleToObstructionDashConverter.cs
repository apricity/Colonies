namespace Wacton.Colonies.UI.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class DoubleToObstructionDashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is double)
            {
                var obstructionLevel = (double)value;
                var gapLength = System.Convert.ToDouble(parameter);

                // do not handle stroke dash if there is no obstruction
                if (Math.Abs(obstructionLevel - 0.0) <= 0.0)
                {
                    return null;
                }

                // empty stroke dash (full line) if perfect obstruction level
                if (Math.Abs(obstructionLevel - 1.0) <= 0.0)
                {
                    return new DoubleCollection(new List<double>());
                }

                var dashLength = gapLength * obstructionLevel;
                return new DoubleCollection(new List<double> { dashLength, gapLength });
            }

            var type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
