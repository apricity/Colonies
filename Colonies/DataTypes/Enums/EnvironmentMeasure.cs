namespace Wacton.Colonies.DataTypes.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public class EnvironmentMeasure : Enumeration, IMeasure
    {
        public static readonly EnvironmentMeasure Pheromone = new EnvironmentMeasure(0, "Pheromone", WeatherType.None, organism => { });
        public static readonly EnvironmentMeasure Nutrient = new EnvironmentMeasure(1, "Nutrient", WeatherType.None, organism => { });
        public static readonly EnvironmentMeasure Mineral = new EnvironmentMeasure(2, "Mineral", WeatherType.None, organism => { });
        public static readonly EnvironmentMeasure Damp = new EnvironmentMeasure(3, "Damp", WeatherType.Damp, organism => organism.OverloadSound());
        public static readonly EnvironmentMeasure Heat = new EnvironmentMeasure(4, "Heat", WeatherType.Heat, organism => organism.OverloadPheromone());
        public static readonly EnvironmentMeasure Disease = new EnvironmentMeasure(5, "Disease", WeatherType.None, organism => organism.ContractDisease());
        public static readonly EnvironmentMeasure Obstruction = new EnvironmentMeasure(6, "Obstruction", WeatherType.None, organism => { });
        public static readonly EnvironmentMeasure Sound = new EnvironmentMeasure(7, "Sound", WeatherType.None, organism => { });

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

        public Action<IOrganism> OrganismAfflication { get; private set; } 

        private EnvironmentMeasure(int value, string friendlyString, WeatherType weatherTrigger, Action<IOrganism> organismAfflication)
            : base(value, friendlyString)
        {
            this.WeatherTrigger = weatherTrigger;
            this.OrganismAfflication = organismAfflication;
        }
    }
}
