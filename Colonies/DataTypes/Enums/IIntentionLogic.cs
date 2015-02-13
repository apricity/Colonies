namespace Wacton.Colonies.DataTypes.Enums
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

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
