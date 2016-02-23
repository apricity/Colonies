namespace Wacton.Colonies.Domain.Measures
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Weathers;
    using Wacton.Tovarisch.Enum;

    public class EnvironmentMeasure : Enumeration, IMeasure
    {
        public static readonly EnvironmentMeasure Pheromone = new EnvironmentMeasure("Pheromone", WeatherType.None);
        public static readonly EnvironmentMeasure Nutrient = new EnvironmentMeasure("Nutrient", WeatherType.None);
        public static readonly EnvironmentMeasure Mineral = new EnvironmentMeasure("Mineral", WeatherType.None);
        public static readonly EnvironmentMeasure Damp = new EnvironmentMeasure("Damp", WeatherType.Damp);
        public static readonly EnvironmentMeasure Heat = new EnvironmentMeasure("Heat", WeatherType.Heat);
        public static readonly EnvironmentMeasure Disease = new EnvironmentMeasure("Disease", WeatherType.None);
        public static readonly EnvironmentMeasure Obstruction = new EnvironmentMeasure("Obstruction", WeatherType.None);
        public static readonly EnvironmentMeasure Sound = new EnvironmentMeasure("Sound", WeatherType.None);

        private static int counter;

        public static IEnumerable<EnvironmentMeasure> HazardousMeasures()
        {
            return new List<EnvironmentMeasure> { Heat, Damp, Disease };
        }

        public bool IsHazardous => HazardousMeasures().Contains(this);
        public WeatherType WeatherTrigger { get; private set; }

        private EnvironmentMeasure(string friendlyString, WeatherType weatherTrigger)
            : base(counter++, friendlyString)
        {
            this.WeatherTrigger = weatherTrigger;
        }
    }
}
