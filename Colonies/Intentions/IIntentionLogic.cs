namespace Wacton.Colonies.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Measures;
    using Wacton.Colonies.Organism;

    public interface IIntentionLogic
    {
        Inventory AssociatedIntenvory { get; }

        Dictionary<EnvironmentMeasure, double> EnvironmentBias { get; }

        bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);

        IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);

        bool CanInteractOrganism(IOrganismState organismState);

        IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState);

    }
}
