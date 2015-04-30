namespace Wacton.Colonies.Domain.Organisms
{
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

    public class Queen : Organism
    {
        public Queen(string name, Color color) : base(name, color, new QueenLogic())
        {
        }

        private class QueenLogic : IOrganismLogic
        {
            public Inventory PreferredInventory
            {
                get
                {
                    return Inventory.Spawn;
                }
            }

            public bool IsSounding(IOrganismState organismState)
            {
                return organismState.CurrentIntention.Equals(Intention.Reproduce);
            }

            public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
            {
                if (organismState.GetLevel(OrganismMeasure.Health) < 0.33)
                {
                    return Intention.Eat;
                }

                if (organismState.GetLevel(OrganismMeasure.Inventory).Equals(1.0))
                {
                    return Intention.Birth;
                }

                return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral) < 1.0 ? Intention.Nest : Intention.Reproduce;
            }
        }
    }
}
