namespace Wacton.Colonies.Domain.Organisms
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

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