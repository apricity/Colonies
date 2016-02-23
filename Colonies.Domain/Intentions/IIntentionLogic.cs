namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public interface IIntentionLogic
    {
        Inventory AssociatedIntenvory { get; }

        Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; }

        bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment);

        IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment);

        bool CanPerformInteraction(IOrganismState organismState);

        IntentionAdjustments EffectsOfInteraction(IOrganismState organismState, IOrganismState otherOrganismState);
    }
}
