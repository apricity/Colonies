namespace Wacton.Colonies.Models.DataAgents
{
    using System.Linq;

    using Wacton.Colonies.Models.Interfaces;

    public class ActionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly Distributor distributor;

        public ActionPhase(EcosystemData ecosystemData, Distributor distributor)
        {
            this.ecosystemData = ecosystemData;
            this.distributor = distributor;
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