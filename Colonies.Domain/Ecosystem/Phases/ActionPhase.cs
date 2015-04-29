namespace Wacton.Colonies.Domain.Ecosystem.Phases
{
    using System.Linq;

    using Wacton.Colonies.Domain.Ecosystem.Modification;

    public class ActionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;

        public ActionPhase(EcosystemData ecosystemData)
        {
            this.ecosystemData = ecosystemData;
        }

        public void Execute()
        {
            this.PerformActions();
        }

        private void PerformActions()
        {
            foreach (var organismCoordinate in this.ecosystemData.AliveOrganismCoordinates().ToList())
            {
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                var environment = this.ecosystemData.GetEnvironment(organismCoordinate);

                if (organism.CanAct(environment))
                {
                    var adjustments = organism.ActionEffects(environment);
                    this.ecosystemData.AdjustLevels(organismCoordinate, adjustments);
                }
            }
        }
    }
}