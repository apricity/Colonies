namespace Wacton.Colonies.Models.Interfaces
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        bool IsAlive { get; }

        Inventory Inventory { get; }
    }
}