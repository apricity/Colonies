namespace Wacton.Colonies.Plugin
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class KamikazeLogic : IOrganismLogic
    {
        public string Description => "Kamikaze!";

        public bool IsSounding(IOrganismState organismState)
        {
            return true;
        }

        public Intention DecideIntention(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return Intention.Build;
        }
    }
}
