namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class EnvironmentInteraction : IEcosystemStage
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;

        public EnvironmentInteraction(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
        }

        public void Execute()
        {
            // remove sound distribution before refreshing intentions, insert them again afterwards if still calling
            this.RemoveSoundDistribution();
            this.ecosystemData.RefreshOrganismIntentions();
            this.PerformInteractions();
            this.InsertSoundDistribution();
        }

        private void RemoveSoundDistribution()
        {
            foreach (var organismCoordinate in this.ecosystemData.CallingOrganismCoordinates())
            {
                this.environmentMeasureDistributor.RemoveDistribution(organismCoordinate, EnvironmentMeasure.Sound);
            }
        }

        private void InsertSoundDistribution()
        {
            foreach (var organismCoordinate in this.ecosystemData.CallingOrganismCoordinates())
            {
                this.environmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Sound);
            }
        }

        private void PerformInteractions()
        {
            foreach (var organismCoordinate in this.ecosystemData.AliveOrganismCoordinates())
            {
                this.InventoryNutrientInteraction(organismCoordinate);
                this.EnvironmentNutrientInteraction(organismCoordinate);
                this.EnvironmentMineralInteraction(organismCoordinate);
                this.EnvironmentHazardInteraction(organismCoordinate);
            }
        }

        private void InventoryNutrientInteraction(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            if (!organism.Intention.Equals(Intention.Eat) || !organism.Inventory.Equals(Inventory.Nutrient))
            {
                return;
            }

            var availableInventoryNutrient = organism.GetLevel(OrganismMeasure.Inventory);
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);

            this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Health, inventoryNutrientTaken);
            this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, -inventoryNutrientTaken);
        }

        private void EnvironmentNutrientInteraction(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
            var availableNutrient = environment.GetLevel(EnvironmentMeasure.Nutrient);
            if (availableNutrient.Equals(0.0))
            {
                return;
            }

            if (organism.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, nutrientTaken);
                this.ecosystemData.AdjustLevel(organismCoordinate, EnvironmentMeasure.Nutrient, -nutrientTaken);
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Health, nutrientTaken);
                this.ecosystemData.AdjustLevel(organismCoordinate, EnvironmentMeasure.Nutrient, -nutrientTaken);
            }
        }

        private void EnvironmentMineralInteraction(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
            var availableMineral = environment.GetLevel(EnvironmentMeasure.Mineral);
            if (availableMineral.Equals(0.0))
            {
                return;
            }

            if (organism.Intention.Equals(Intention.Mine))
            {
                var desiredMineral = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                var mineralTaken = Math.Min(desiredMineral, availableMineral);
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, mineralTaken);
                this.ecosystemData.AdjustLevel(organismCoordinate, EnvironmentMeasure.Mineral, -mineralTaken);
            }

            // reproduction requirements are attached to the IsCalling property
            // organism inventory of "spawn" is filled, ready for the organism to place it
            if (organism.Intention.Equals(Intention.Reproduce) && !organism.IsCalling)
            {
                var mineralTaken = availableMineral;
                this.ecosystemData.AdjustLevel(organismCoordinate, EnvironmentMeasure.Mineral, -mineralTaken);
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, 1.0);
            }
        }

        private void EnvironmentHazardInteraction(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
            if (!organism.Intention.Equals(Intention.Build)
                || organism.GetLevel(OrganismMeasure.Inventory) < 1.0)
            {
                return;
            }

            var hazardousMeasurements = environment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            if (hazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, -1.0);
                this.ecosystemData.AdjustLevel(organismCoordinate, EnvironmentMeasure.Obstruction, 1.0);
            }
        }
    }
}