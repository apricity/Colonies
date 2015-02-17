namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class ActionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;
        private readonly Dictionary<EnvironmentMeasure, Action<IOrganism>> harmfulEnvironmentActions;  

        public ActionPhase(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;

            this.harmfulEnvironmentActions = new Dictionary<EnvironmentMeasure, Action<IOrganism>>
            {
                { EnvironmentMeasure.Heat, organism => organism.OverloadPheromone() },
                { EnvironmentMeasure.Damp, organism => organism.OverloadSound() },
                { EnvironmentMeasure.Disease, organism => organism.ContractDisease() },
            };
        }

        public void Execute()
        {
            this.ecosystemData.RefreshOrganismIntentions();
            this.PerformInteractions();
        }

        private void PerformInteractions()
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

                // TODO: do hazard interactions on last phase instead (after movement phase)?
                this.HazardInteraction(organism, environment);
            }
        }

        private void HazardInteraction(IOrganism organism, IEnvironment environment)
        {
            if (!environment.IsHarmful)
            {
                return;
            }

            foreach (var harmfulMeasure in environment.HarmfulMeasures)
            {
                this.harmfulEnvironmentActions[harmfulMeasure].Invoke(organism);
            }
        }
    }
}