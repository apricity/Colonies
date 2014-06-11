namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Interfaces;

    public class Intention : Enumeration
    {
        public static readonly Intention Eat = new Intention(
            0, "Eat",
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 10 },
                    { EnvironmentMeasure.Pheromone, 10 }
                });

        //public static readonly Intention Gather = new Intention(1, "Gather");
        //public static readonly Intention Feed = new Intention(2, "Feed");
        //public static readonly Intention Extract = new Intention(3, "Extract");
        //public static readonly Intention Defend = new Intention(4, "Defend");
        //public static readonly Intention Search = new Intention(5, "Search");
        //public static readonly Intention Nest = new Intention(6, "Nest");

        private Intention(int value, string friendlyString, Dictionary<EnvironmentMeasure, double> environmentBiases)
            : base(value, friendlyString)
        {
        }
    }
}
