namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        Inventory Inventory { get; }

        Intention Intention { get; }

        bool IsAlive { get; }

        bool IsReproducing { get; }

        bool NeedsAssistance { get; }

        Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        void RefreshIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);
    }
}