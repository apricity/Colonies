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

        bool CanInteractOrganism(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);

        IntentionAdjustments InteractOrganismAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);

    }
}
