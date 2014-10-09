namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    public class Intention : Enumeration
    {
        public static readonly Intention None = new Intention(0, "None", null,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 0 },
                    { EnvironmentMeasure.Pheromone, 0 },
                    { EnvironmentMeasure.Sound, 0 },
                    { EnvironmentMeasure.Damp, 0 },
                    { EnvironmentMeasure.Heat, 0 },
                    { EnvironmentMeasure.Poison, 0 }
                });

        public static readonly Intention Eat = new Intention(1, "Eat", null,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 10 },
                    { EnvironmentMeasure.Pheromone, 10 },
                    { EnvironmentMeasure.Sound, 10 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Harvest = new Intention(2, "Harvest", Inventory.Nutrient,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 10 },
                    { EnvironmentMeasure.Pheromone, 10 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Nourish = new Intention(3, "Nourish", Inventory.Nutrient,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Sound, 25 },
                    { EnvironmentMeasure.Pheromone, -25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Mine = new Intention(4, "Mine", Inventory.Mineral,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Mineral, 25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 },
                    { EnvironmentMeasure.Obstruction, -50 }
                });

        public static readonly Intention Build = new Intention(5, "Build", Inventory.Mineral,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Sound, 50 },
                    { EnvironmentMeasure.Damp, 10 },
                    { EnvironmentMeasure.Heat, 10 },
                    { EnvironmentMeasure.Poison, 25 },
                    { EnvironmentMeasure.Obstruction, -50 }
                });

        public static readonly Intention Nest = new Intention(6, "Nest", Inventory.Spawn,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Mineral, 25 },
                    { EnvironmentMeasure.Damp, -10 },
                    { EnvironmentMeasure.Heat, -10 },
                    { EnvironmentMeasure.Poison, -50 }
                });

        public static readonly Intention Dead = new Intention(7, "Dead", null,
            new Dictionary<EnvironmentMeasure, double>
                {
                    { EnvironmentMeasure.Nutrient, 0 },
                    { EnvironmentMeasure.Pheromone, 0 },
                    { EnvironmentMeasure.Sound, 0 },
                    { EnvironmentMeasure.Damp, 0 },
                    { EnvironmentMeasure.Heat, 0 },
                    { EnvironmentMeasure.Poison, 0 }
                });

        public static readonly Intention Reproduce = new Intention(7, "Reproduce", Inventory.Spawn,
            new Dictionary<EnvironmentMeasure, double>());

        public Inventory RequiredInventory { get; private set; }
        public Dictionary<EnvironmentMeasure, double> EnvironmentBiases { get; private set; } 

        private bool RequiresInventory
        {
            get
            {
                return this.RequiredInventory != null;
            }
        }

        private Intention(int value, string friendlyString, Inventory requiredInventory, Dictionary<EnvironmentMeasure, double> environmentBiases)
            : base(value, friendlyString)
        {
            this.RequiredInventory = requiredInventory;

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

        public bool IsCompatibleWith(Inventory inventory)
        {
            return !this.RequiresInventory || this.RequiredInventory.Equals(inventory);
        }
    }
}
