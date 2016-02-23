namespace Wacton.Colonies.Domain.Organisms
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

    public class DummyLogic : IOrganismLogic
    {
        public string Description => "Dummy";

        public bool IsSounding(IOrganismState organismState)
        {
            return false;
        }

        public Intention DecideIntention(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return Intention.Eat;
        }
    }
}
