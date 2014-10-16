namespace Wacton.Colonies.Models.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class OrganismMovement : IEcosystemStage, IBiased<OrganismMeasure>
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemRates ecosystemRates;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;

        public Dictionary<OrganismMeasure, double> MeasureBiases { get; private set; }

        public OrganismMovement(EcosystemData ecosystemData, EcosystemRates ecosystemRates, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemRates = ecosystemRates;
            this.environmentMeasureDistributor = environmentMeasureDistributor;

            this.MeasureBiases = new Dictionary<OrganismMeasure, double>
                                     {
                                         { OrganismMeasure.Health, 1.0 },
                                         { OrganismMeasure.Inventory, 0.0 }
                                     };
        }

        public void Execute()
        {
            // TODO: pull movement logic into this class, break up ecosystem logic
            var desiredOrganismCoordinates = EcosystemLogic.GetDesiredCoordinates(this.ecosystemData);
            var movedOrganismCoordinates = EcosystemLogic.ResolveOrganismHabitats(this.ecosystemData, desiredOrganismCoordinates, new List<IOrganism>(), this);

            this.IncreasePheromoneLevels();
            this.IncreaseMineralLevels();

            foreach (var movedOrganismCoordinate in movedOrganismCoordinates)
            {
                this.ecosystemData.MoveOrganism(movedOrganismCoordinate.Key, movedOrganismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            this.ecosystemData.AdjustLevels(obstructedCoordinates, EnvironmentMeasure.Obstruction, -this.ecosystemRates.DecreasingRates[EnvironmentMeasure.Obstruction]);

            this.InsertSoundDistribution();
        }

        private void IncreasePheromoneLevels()
        {
            var organismCoordinates = this.ecosystemData.DepositingPheromoneOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, EnvironmentMeasure.Pheromone, this.ecosystemRates.IncreasingRates[EnvironmentMeasure.Pheromone]);
        }

        private void IncreaseMineralLevels()
        {
            // only increase mineral where the terrain is not harmful (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var organismCoordinates = this.ecosystemData.OrganismCoordinates()
                .Where(coordinate => !this.ecosystemData.IsHarmful(coordinate)).ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, EnvironmentMeasure.Mineral, this.ecosystemRates.IncreasingRates[EnvironmentMeasure.Mineral]);
        }

        private void InsertSoundDistribution()
        {
            foreach (var organismCoordinate in this.ecosystemData.NeedingAssistanceOrganismCoordinates())
            {
                this.environmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Sound);
            }
        }
    }
}