namespace Wacton.Colonies.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;

    public class EcosystemRates
    {
        public Dictionary<IMeasure, double> IncreasingRates { get; private set; } 
        public Dictionary<IMeasure, double> DecreasingRates { get; private set; } 
        public Dictionary<EnvironmentMeasure, HazardRate> HazardRates { get; private set; }

        public EcosystemRates()
        {
            this.IncreasingRates = new Dictionary<IMeasure, double>
                {
                    { EnvironmentMeasure.Pheromone, 1 / 10.0 },
                    { EnvironmentMeasure.Nutrient, 1 / 500.0 },
                    { EnvironmentMeasure.Mineral, 1 / 100.0 }
                };

            this.DecreasingRates = new Dictionary<IMeasure, double>
                {
                    { OrganismMeasure.Health, 1 / 750.0 },
                    { EnvironmentMeasure.Pheromone, 1 / 300.0 },
                    { EnvironmentMeasure.Obstruction, 1 / 5.0 }
                };

            this.HazardRates = new Dictionary<EnvironmentMeasure, HazardRate>
                {
                    { EnvironmentMeasure.Damp, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Heat, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Poison, new HazardRate(0.0, 1 / 50.0, 1 / 50.0) }
                };
        }
    }
}
