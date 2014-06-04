namespace Wacton.Colonies.Ancillary
{
    using Wacton.Colonies.Interfaces;

    public class EnvironmentMeasure : Enumeration, IMeasure
    {
        public static readonly EnvironmentMeasure Pheromone = new EnvironmentMeasure(0, "Pheromone");
        public static readonly EnvironmentMeasure Nutrient = new EnvironmentMeasure(1, "Nutrient");
        public static readonly EnvironmentMeasure Mineral = new EnvironmentMeasure(2, "Mineral");
        public static readonly EnvironmentMeasure Damp = new EnvironmentMeasure(3, "Damp");
        public static readonly EnvironmentMeasure Heat = new EnvironmentMeasure(4, "Heat");
        public static readonly EnvironmentMeasure Poison = new EnvironmentMeasure(5, "Poison");
        public static readonly EnvironmentMeasure Obstruction = new EnvironmentMeasure(6, "Obstruction");

        private EnvironmentMeasure(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }
    }
}
