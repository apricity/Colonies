namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    public class Intention : Enumeration
    {
        public static readonly Intention Eat = new Intention(0, "Eat",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 10 },
                    { EnvironmentMeasure.Pheromone, 10 },
                    { EnvironmentMeasure.Sound, 10 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Harvest = new Intention(1, "Harvest",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 10 },
                    { EnvironmentMeasure.Pheromone, 10 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Nourish = new Intention(2, "Nourish",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Sound, 25 },
                    { EnvironmentMeasure.Pheromone, -25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Mine = new Intention(3, "Mine",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Mineral, 25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 },
                    { EnvironmentMeasure.Obstruction, -50 }
                });

        public static readonly Intention Build = new Intention(4, "Build",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Sound, 50 },
                    { EnvironmentMeasure.Damp, 10 },
                    { EnvironmentMeasure.Heat, 10 },
                    { EnvironmentMeasure.Poison, 25 },
                    { EnvironmentMeasure.Obstruction, -50 }
                });

        public static readonly Intention Nest = new Intention(5, "Nest",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Mineral, 25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Reproduce = new Intention(6, "Reproduce",
            new Dictionary<EnvironmentMeasure, double>());

        public Dictionary<EnvironmentMeasure, double> EnvironmentBiases { get; private set; } 

        private Intention(int value, string friendlyString, Dictionary<EnvironmentMeasure, double> environmentBiases)
            : base(value, friendlyString)
        {
            this.EnvironmentBiases = new Dictionary<EnvironmentMeasure, double>();
            foreach (var environmentMeasure in Enumeration.GetAll<EnvironmentMeasure>())
            {
                this.EnvironmentBiases.Add(environmentMeasure, 0);
            }

            foreach (var environmentBias in environmentBiases)
            {
                this.EnvironmentBiases[environmentBias.Key] = environmentBias.Value;
            }
        }
    }
}
