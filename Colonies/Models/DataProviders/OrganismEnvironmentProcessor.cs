namespace Wacton.Colonies.Models.DataProviders
{
    using System;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public class OrganismEnvironmentProcessor
    {
        private readonly EcosystemData ecosystemData;

        public OrganismEnvironmentProcessor(EcosystemData ecosystemData)
        {
            this.ecosystemData = ecosystemData;
        }

        public void Process(Coordinate organismCoordinate)
        {
            this.ProcessInventoryNutrient(organismCoordinate);
            this.ProcessEnvironmentNutrient(organismCoordinate);
            this.ProcessEnvironmentMineral(organismCoordinate);
            this.ProcessEnvironmentHazards(organismCoordinate);
        }

        private void ProcessInventoryNutrient(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            if (!organism.Intention.Equals(Intention.Eat) || !organism.Inventory.Equals(Inventory.Nutrient))
            {
                return;
            }

            var availableInventoryNutrient = organism.GetLevel(OrganismMeasure.Inventory);
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);

            this.ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Health, inventoryNutrientTaken);
            this.ecosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Inventory, inventoryNutrientTaken);
        }

        private void ProcessEnvironmentNutrient(Coordinate organismCoordinate)
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
                this.ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Inventory, nutrientTaken);
                this.ecosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Nutrient, nutrientTaken);
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Health, nutrientTaken);
                this.ecosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Nutrient, nutrientTaken);
            }
        }

        private void ProcessEnvironmentMineral(Coordinate organismCoordinate)
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
                this.ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Inventory, mineralTaken);
                this.ecosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Mineral, mineralTaken);
            }

            // reproduction requirements (first pass: mineral level 1.0, health level 0.75)
            if (organism.Intention.Equals(Intention.Reproduce)
                && environment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0)
                && organism.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                // TODO: create the result of using the mineral during reproduction!  a child organism?!
                var mineralTaken = availableMineral;
                this.ecosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Mineral, mineralTaken);
            }
        }

        private void ProcessEnvironmentHazards(Coordinate organismCoordinate)
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
                this.ecosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Inventory, 1.0);
                this.ecosystemData.IncreaseLevel(organismCoordinate, EnvironmentMeasure.Obstruction, 1.0);
            }
        }
    }
}