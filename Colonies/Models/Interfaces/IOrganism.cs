namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        bool IsAlive { get; }

        bool IsReproducing { get; }

        string IntentionString { get; }

        Measurement<EnvironmentMeasure> Inventory { get; }

        Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment);
    }
}