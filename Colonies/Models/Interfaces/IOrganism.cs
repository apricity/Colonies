namespace Wacton.Colonies.Models.Interfaces
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        Inventory Inventory { get; }

        Intention Intention { get; }

        bool IsAlive { get; }

        bool IsCalling { get; }

        void RefreshIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);
    }
}