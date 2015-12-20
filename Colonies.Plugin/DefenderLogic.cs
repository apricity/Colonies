namespace Wacton.Colonies.Plugin
{
    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class DefenderLogic : IOrganismLogic
    {
        public string Description => "Defender";

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
