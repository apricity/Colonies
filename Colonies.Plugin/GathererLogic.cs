namespace Wacton.Colonies.Plugin
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class GathererLogic : IOrganismLogic 
    {
        public string Description => "Gatherer";

        public bool IsSounding(IOrganismState organismState)
        {
            return false;
        }

        public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (organismState.GetLevel(OrganismMeasure.Health) < 0.25)
            {
                return Intention.Eat;
            }

            return organismState.GetLevel(OrganismMeasure.Inventory) < 0.75 ? Intention.Harvest : Intention.Nourish;
        }
    }
}
