namespace Wacton.Colonies.Plugin
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class Kamikaze : IOrganismLogic
    {
        public string Description
        {
            get
            {
                return "Kamikaze!";
            }
        }

        public bool IsSounding(IOrganismState organismState)
        {
            return false;
        }

        public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return Intention.Build;
        }
    }
}
