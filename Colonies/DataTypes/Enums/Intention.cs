namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    public class Intention : Enumeration
    {
        public static readonly Intention None = new Intention(0, "None", new IntentionEatLogic());
        public static readonly Intention Eat = new Intention(1, "Eat", new IntentionEatLogic());
        public static readonly Intention Harvest = new Intention(2, "Harvest", new IntentionEatLogic());
        public static readonly Intention Nourish = new Intention(3, "Nourish", new IntentionEatLogic());
        public static readonly Intention Mine = new Intention(4, "Mine", new IntentionEatLogic());
        public static readonly Intention Build = new Intention(5, "Build", new IntentionEatLogic());
        public static readonly Intention Nest = new Intention(6, "Nest", new IntentionEatLogic());
        public static readonly Intention Reproduce = new Intention(7, "Reproduce", new IntentionEatLogic());
        public static readonly Intention Birth = new Intention(8, "Birth", new IntentionEatLogic());
        public static readonly Intention Dead = new Intention(9, "Dead", new IntentionEatLogic());

        public IIntentionLogic IntentionLogic { get; private set; }

        public Inventory AssociatedInventory
        {
            get
            {
                return this.IntentionLogic.AssociatedIntenvory;
            }
        }
        public Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; private set; }

        private Intention(int value, string friendlyString, IIntentionLogic intentionLogic)
            : base(value, friendlyString)
        {
            this.IntentionLogic = intentionLogic;

            this.EnvironmentBias = new Dictionary<EnvironmentMeasure, double>();
            foreach (var environmentMeasure in Enumeration.GetAll<EnvironmentMeasure>())
            {
                this.EnvironmentBias.Add(environmentMeasure, 0);
            }

            foreach (var environmentBias in this.IntentionLogic.EnvironmentBias)
            {
                this.EnvironmentBias[environmentBias.Key] = environmentBias.Value;
            }
        }

        public bool HasConflictingInventory(Intention otherIntention)
        {
            if (this.AssociatedInventory == null
                || otherIntention.AssociatedInventory == null)
            {
                return false;
            }

            return !this.AssociatedInventory.Equals(otherIntention.AssociatedInventory);
        }
    }
}
