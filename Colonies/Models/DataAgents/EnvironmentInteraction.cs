namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class EnvironmentInteraction : IEcosystemStage
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;
        private readonly Dictionary<EnvironmentMeasure, Action<IOrganism>> harmfulEnvironmentActions;  

        public EnvironmentInteraction(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
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

                if (organism.CanInteractEnvironment(environment))
                {
                    var adjustments = organism.InteractEnvironmentAdjustments(environment);
                    this.ecosystemData.AdjustLevels(organismCoordinate, adjustments);
                }

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