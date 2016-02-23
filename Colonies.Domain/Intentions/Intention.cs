namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Tovarisch.Enum;

    public class Intention : Enumeration
    {
        public static readonly Intention None = new Intention("None", new NoLogic());
        public static readonly Intention Eat = new Intention("Eat", new EatLogic());
        public static readonly Intention Harvest = new Intention("Harvest", new HarvestLogic());
        public static readonly Intention Nourish = new Intention("Nourish", new NourishLogic());
        public static readonly Intention Mine = new Intention("Mine", new MineLogic());
        public static readonly Intention Build = new Intention("Build", new BuildLogic());
        public static readonly Intention Nest = new Intention("Nest", new NestLogic());
        public static readonly Intention Reproduce = new Intention("Reproduce", new ReproduceLogic());
        public static readonly Intention Birth = new Intention("Birth", new BirthLogic());

        private static int counter;

        private readonly IIntentionLogic logic;
        public Inventory AssociatedInventory => this.logic.AssociatedIntenvory;
        public Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; }

        private Intention(string friendlyString, IIntentionLogic logic)
            : base(counter++, friendlyString)
        {
            this.logic = logic;

            this.EnvironmentBias = new Dictionary<EnvironmentMeasure, double>();
            foreach (var environmentMeasure in Enumeration.GetAll<EnvironmentMeasure>())
            {
                this.EnvironmentBias.Add(environmentMeasure, 0);
            }

            foreach (var environmentBias in this.logic.EnvironmentBias)
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

        public bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return this.logic.CanPerformAction(organismState, measurableEnvironment);
        }

        public IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return this.logic.EffectsOfAction(organismState, measurableEnvironment);
        }

        public bool CanPerformInteraction(IOrganismState organismState)
        {
            return this.logic.CanPerformInteraction(organismState);
        }

        public IntentionAdjustments EffectsOfInteraction(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return this.logic.EffectsOfInteraction(organismState, otherOrganismState);
        }
    }
}
