namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganismState : IMeasurable<OrganismMeasure>
    {
        Intention CurrentIntention { get; }

        Inventory CurrentInventory { get; }

        bool IsAlive { get; }

        bool CanMove { get; }

        bool IsAudible { get; }

        bool IsPheromoneOverloaded { get; }

        bool IsSoundOverloaded { get; }

        bool IsDiseased { get; }

        bool IsInfectious { get; }
    }
}