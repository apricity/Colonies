namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public abstract class ActionIntentionLogic : IIntentionLogic
    {
        public abstract Inventory AssociatedIntenvory { get; }

        public abstract Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; }

        public abstract bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment);

        public abstract IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment);

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
