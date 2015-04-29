namespace Wacton.Colonies.Domain.Organism
{
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

    public class Defender : Organism
    {
        public Defender(string name, Color color) : base(name, color, new DefenderLogic())
        {
        }

        private class DefenderLogic : IOrganismLogic
        {
            public Inventory PreferredInventory
            {
                get
                {
                    return Inventory.Mineral;
                }
            }

            public bool IsSounding(IOrganismState organismState)
            {
                return false;
            }

            public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
            {
                if (organismState.GetLevel(OrganismMeasure.Health) < 0.33)
                {
                    return Intention.Eat;
                }

                return organismState.GetLevel(OrganismMeasure.Inventory) < 1.0 ? Intention.Mine : Intention.Build;
            }
        }
    }
}
