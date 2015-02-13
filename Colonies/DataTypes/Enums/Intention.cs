namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class Intention : Enumeration
    {
        public static readonly Intention None = new Intention(0, "None", new NoLogic());
        public static readonly Intention Eat = new Intention(1, "Eat", new EatLogic());
        public static readonly Intention Harvest = new Intention(2, "Harvest", new HarvestLogic());
        public static readonly Intention Nourish = new Intention(3, "Nourish", new NourishLogic());
        public static readonly Intention Mine = new Intention(4, "Mine", new MineLogic());
        public static readonly Intention Build = new Intention(5, "Build", new BuildLogic());
        public static readonly Intention Nest = new Intention(6, "Nest", new NestLogic());
        public static readonly Intention Reproduce = new Intention(7, "Reproduce", new ReproduceLogic());
        public static readonly Intention Birth = new Intention(8, "Birth", new BirthLogic());
        public static readonly Intention Dead = new Intention(9, "Dead", new NoLogic());

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

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.IntentionLogic.CanInteractEnvironment(measurableEnvironment, organismState);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.IntentionLogic.InteractEnvironmentAdjustments(measurableEnvironment, organismState);
        }

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return this.IntentionLogic.CanInteractOrganism(organismState);
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return this.IntentionLogic.InteractOrganismAdjustments(organismState, otherOrganismState);
        }
    }
}
