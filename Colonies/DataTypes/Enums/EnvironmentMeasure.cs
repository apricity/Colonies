namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Interfaces;

    public class EnvironmentMeasure : Enumeration, IMeasure
    {
        public static readonly EnvironmentMeasure Pheromone = new EnvironmentMeasure(0, "Pheromone");
        public static readonly EnvironmentMeasure Nutrient = new EnvironmentMeasure(1, "Nutrient");
        public static readonly EnvironmentMeasure Mineral = new EnvironmentMeasure(2, "Mineral");
        public static readonly EnvironmentMeasure Damp = new EnvironmentMeasure(3, "Damp");
        public static readonly EnvironmentMeasure Heat = new EnvironmentMeasure(4, "Heat");
        public static readonly EnvironmentMeasure Poison = new EnvironmentMeasure(5, "Poison");
        public static readonly EnvironmentMeasure Obstruction = new EnvironmentMeasure(6, "Obstruction");

        public static IEnumerable<EnvironmentMeasure> PotentialHazards()
        {
            return new List<EnvironmentMeasure> { Heat, Damp, Poison };
        }

        private EnvironmentMeasure(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }

        public bool IsPotentialHazard()
        {
            return PotentialHazards().Contains(this);
        }
    }
}
