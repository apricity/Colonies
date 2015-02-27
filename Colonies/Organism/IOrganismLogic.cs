namespace Wacton.Colonies.Organism
{
    using Wacton.Colonies.Intentions;
    using Wacton.Colonies.Measures;

    public interface IOrganismLogic
    {
        Inventory PreferredInventory { get; }

        bool IsSounding(IOrganismState organismState);

        Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);
    }
}