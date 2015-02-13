namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    public static class IntentionBiases
    {
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
