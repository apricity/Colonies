namespace Wacton.Colonies.Models.DataAgents
{
    using System.Linq;

    using Wacton.Colonies.Models.Interfaces;

    public class ActionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;

        public ActionPhase(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
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