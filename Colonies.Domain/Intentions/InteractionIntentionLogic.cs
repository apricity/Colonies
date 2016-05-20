namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public abstract class InteractionIntentionLogic : IIntentionLogic
    {
        public abstract Inventory AssociatedIntenvory { get; }

        public abstract Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; }

        public bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return false;
        }

        public IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return new IntentionAdjustments();
        }

        public abstract bool CanPerformInteraction(IOrganismState organismState);

        public abstract IntentionAdjustments EffectsOfInteraction(IOrganismState organismState, IOrganismState otherOrganismState);
    }
}
