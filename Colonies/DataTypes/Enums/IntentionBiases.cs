namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    public static class IntentionBiases
    {
        public static Dictionary<EnvironmentMeasure, double> NoBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Nutrient, 0 },
                           { EnvironmentMeasure.Pheromone, 0 },
                           { EnvironmentMeasure.Sound, 0 },
                           { EnvironmentMeasure.Damp, 0 },
                           { EnvironmentMeasure.Heat, 0 },
                           { EnvironmentMeasure.Disease, 0 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> EatBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Nutrient, 10 },
                           { EnvironmentMeasure.Pheromone, 10 },
                           { EnvironmentMeasure.Sound, 10 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> HarvestBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Nutrient, 10 },
                           { EnvironmentMeasure.Pheromone, 10 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> NourishBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Sound, 25 },
                           { EnvironmentMeasure.Pheromone, -25 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> MineBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Mineral, 25 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 },
                           { EnvironmentMeasure.Obstruction, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> BuildBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Sound, 50 },
                           { EnvironmentMeasure.Damp, 10 },
                           { EnvironmentMeasure.Heat, 10 },
                           { EnvironmentMeasure.Disease, 25 },
                           { EnvironmentMeasure.Obstruction, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> NestBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Mineral, 25 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
        }

        public static Dictionary<EnvironmentMeasure, double> BirthBias()
        {
            return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
        }
    }
}
