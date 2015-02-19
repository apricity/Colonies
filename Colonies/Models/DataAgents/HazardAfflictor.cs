namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public class HazardAfflictor
    {
        private EcosystemData EcosystemData { get; set; }
        private EnvironmentMeasureDistributor EnvironmentMeasureDistributor { get; set; }
        private readonly Dictionary<EnvironmentMeasure, Action<Coordinate>> hazardActions;

        public HazardAfflictor(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.EcosystemData = ecosystemData;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;

            this.hazardActions
                = new Dictionary<EnvironmentMeasure, Action<Coordinate>>
                {
                    { EnvironmentMeasure.Heat, this.HeatHazard },
                    { EnvironmentMeasure.Damp, this.DampHazard },
                    { EnvironmentMeasure.Disease, this.DiseaseHazard }
                };
        }

        public void HazardAffliction(Coordinate organismCoordinate)
        {
            var environment = this.EcosystemData.GetEnvironment(organismCoordinate);
            if (!environment.IsHarmful)
            {
                return;
            }

            foreach (var harmfulMeasure in environment.HarmfulMeasures)
            {
                this.hazardActions[harmfulMeasure].Invoke(organismCoordinate);
            }
        }

        private void HeatHazard(Coordinate organismCoordinate)
        {
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.OverloadPheromone();
        }

        private void DampHazard(Coordinate organismCoordinate)
        {
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.OverloadSound();
            this.EnvironmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Sound);
        }

        private void DiseaseHazard(Coordinate organismCoordinate)
        {
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.ContractDisease();
            this.EnvironmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Disease); // TODO: not needed? already exists, otherwise wouldn't get diseased
        }
    }
}
