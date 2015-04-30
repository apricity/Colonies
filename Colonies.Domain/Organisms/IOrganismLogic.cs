namespace Wacton.Colonies.Domain.Organisms
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

    public interface IOrganismLogic
    {
        Inventory PreferredInventory { get; }

        bool IsSounding(IOrganismState organismState);

        Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);
    }
}