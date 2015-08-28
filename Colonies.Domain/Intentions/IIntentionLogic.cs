namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

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
