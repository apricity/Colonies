namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Gatherer : Organism
    {
        public Gatherer(string name, Color color) : base(name, color, new GathererLogic())
        { 
        }

        private class GathererLogic : IOrganismLogic 
        {
            public Inventory PreferredInventory
            {
                get
                {
                    return Inventory.Nutrient;
                }
            }

            public bool IsSounding(IOrganismState organismState)
            {
                return false;
            }

            public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
            {
                return organismState.GetLevel(OrganismMeasure.Inventory) < 0.75 ? Intention.Harvest : Intention.Nourish;
            }
        }
    }
}
