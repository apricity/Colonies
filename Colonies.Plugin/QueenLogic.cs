namespace Wacton.Colonies.Plugin
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class QueenLogic : IOrganismLogic
    {
        public string Description => "Queen";

        public bool IsSounding(IOrganismState organismState)
        {
            return organismState.CurrentIntention.Equals(Intention.Reproduce);
        }

        public Intention DecideIntention(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
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
