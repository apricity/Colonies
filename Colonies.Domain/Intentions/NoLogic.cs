namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class NoLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory => Inventory.None;
        public Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Nutrient, 0 },
                { EnvironmentMeasure.Pheromone, 0 },
                { EnvironmentMeasure.Sound, 0 },
                { EnvironmentMeasure.Damp, 0 },
                { EnvironmentMeasure.Heat, 0 },
                { EnvironmentMeasure.Disease, 0 },
                { EnvironmentMeasure.Obstruction, 0 },
            };

        public bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return false;
        }

        public IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return new IntentionAdjustments();
        }

        public bool CanPerformInteraction(IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments EffectsOfInteraction(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }
    }
}
