namespace Wacton.Colonies.Domain.Measures
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Weathers;
    using Wacton.Tovarisch.Enum;

    public class EnvironmentMeasure : Enumeration, IMeasure
    {
        public static readonly EnvironmentMeasure Pheromone = new EnvironmentMeasure(0, "Pheromone", WeatherType.None);
        public static readonly EnvironmentMeasure Nutrient = new EnvironmentMeasure(1, "Nutrient", WeatherType.None);
        public static readonly EnvironmentMeasure Mineral = new EnvironmentMeasure(2, "Mineral", WeatherType.None);
        public static readonly EnvironmentMeasure Damp = new EnvironmentMeasure(3, "Damp", WeatherType.Damp);
        public static readonly EnvironmentMeasure Heat = new EnvironmentMeasure(4, "Heat", WeatherType.Heat);
        public static readonly EnvironmentMeasure Disease = new EnvironmentMeasure(5, "Disease", WeatherType.None);
        public static readonly EnvironmentMeasure Obstruction = new EnvironmentMeasure(6, "Obstruction", WeatherType.None);
        public static readonly EnvironmentMeasure Sound = new EnvironmentMeasure(7, "Sound", WeatherType.None);

        public static IEnumerable<EnvironmentMeasure> HazardousMeasures()
        {
            return new List<EnvironmentMeasure> { Heat, Damp, Disease };
        }

        public bool IsHazardous
        {
            get
            {
                return HazardousMeasures().Contains(this);
            }
        }

        public WeatherType WeatherTrigger { get; private set; }

        private EnvironmentMeasure(int value, string friendlyString, WeatherType weatherTrigger)
            : base(value, friendlyString)
        {
            this.WeatherTrigger = weatherTrigger;
        }
    }
}
