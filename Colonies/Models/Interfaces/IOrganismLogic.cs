namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganismLogic
    {
        Inventory PreferredInventory { get; }

        bool IsSounding(IOrganismState organismState);

        Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState);
    }
}