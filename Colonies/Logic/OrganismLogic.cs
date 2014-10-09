namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Models;
    using Wacton.Colonies.Models.DataProviders;
    using Wacton.Colonies.Models.Interfaces;

    public static class OrganismLogic
    {
        public static Dictionary<EnvironmentMeasure, double> ProcessMeasurableEnvironment(Coordinate organismCoordinate, EcosystemData ecosystemData)
        {
            var organism = ecosystemData.GetOrganism(organismCoordinate);
            var environment = ecosystemData.GetEnvironment(organismCoordinate);
            var modifiedEnvironmentMeasures = new Dictionary<EnvironmentMeasure, double>();

            if (organism.Intention.Equals(Intention.Eat))
            {
                ConsumeInventoryNutrients(organismCoordinate, ecosystemData);
            }

            var nutrientTaken = ProcessNutrient(organismCoordinate, ecosystemData);
            var mineralTaken = ProcessMineral(organismCoordinate, ecosystemData);
            var obstructionCreated = ProcessHazards(organismCoordinate, ecosystemData);

            if (nutrientTaken > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Nutrient, -nutrientTaken);
            }

            if (mineralTaken > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Mineral, -mineralTaken);
            }

            if (obstructionCreated > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Obstruction, obstructionCreated);
            }

            return modifiedEnvironmentMeasures;
        }

        public static Dictionary<EnvironmentMeasure, double> ProcessMeasurableEnvironment(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var modifiedEnvironmentMeasures = new Dictionary<EnvironmentMeasure, double>();

            if (organism.Intention.Equals(Intention.Eat))
            {
                ConsumeInventoryNutrients(organism);
            }

            var nutrientTaken = ProcessNutrient(organism, measurableEnvironment);
            var mineralTaken = ProcessMineral(organism, measurableEnvironment);
            var obstructionCreated = ProcessHazards(organism, measurableEnvironment);

            if (nutrientTaken > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Nutrient, -nutrientTaken);
            }

            if (mineralTaken > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Mineral, -mineralTaken);
            }

            if (obstructionCreated > 0.0)
            {
                modifiedEnvironmentMeasures.Add(EnvironmentMeasure.Obstruction, obstructionCreated);
            }

            return modifiedEnvironmentMeasures;
        }

        private static void ConsumeInventoryNutrients(Coordinate organismCoordinate, EcosystemData ecosystemData)
        {
            var organism = ecosystemData.GetOrganism(organismCoordinate);
            if (!organism.Inventory.Equals(Inventory.Nutrient))
            {
                return;
            }

            var availableInventoryNutrient = organism.GetLevel(OrganismMeasure.Inventory);
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
            ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Health, inventoryNutrientTaken);
            ecosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Inventory, inventoryNutrientTaken);
        }

        private static void ConsumeInventoryNutrients(Organism organism)
        {
            if (!organism.Inventory.Equals(Inventory.Nutrient))
            {
                return;
            }

            var availableInventoryNutrient = organism.GetLevel(OrganismMeasure.Inventory);
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
            organism.IncreaseLevel(OrganismMeasure.Health, inventoryNutrientTaken);
            organism.DecreaseLevel(OrganismMeasure.Inventory, inventoryNutrientTaken);
        }

        private static double ProcessNutrient(Coordinate organismCoordinate, EcosystemData ecosystemData)
        {
            var organism = ecosystemData.GetOrganism(organismCoordinate);
            var environment = ecosystemData.GetEnvironment(organismCoordinate);

            var availableNutrient = environment.GetLevel(EnvironmentMeasure.Nutrient);
            var nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (organism.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Inventory, nutrientTaken);
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Health, nutrientTaken);
            }

            return nutrientTaken;
        }

        private static double ProcessNutrient(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var availableNutrient = measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient);
            var nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (organism.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                organism.IncreaseLevel(OrganismMeasure.Inventory, nutrientTaken);
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                organism.IncreaseLevel(OrganismMeasure.Health, nutrientTaken);
            }

            return nutrientTaken;
        }

        private static double ProcessMineral(Coordinate organismCoordinate, EcosystemData ecosystemData)
        {
            var organism = ecosystemData.GetOrganism(organismCoordinate);
            var environment = ecosystemData.GetEnvironment(organismCoordinate);

            var availableMineral = environment.GetLevel(EnvironmentMeasure.Mineral);
            var mineralTaken = 0.0;

            if (availableMineral.Equals(0.0))
            {
                return mineralTaken;
            }

            if (organism.Intention.Equals(Intention.Mine))
            {
                var desiredMineral = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                mineralTaken = Math.Min(desiredMineral, availableMineral);
                ecosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Inventory, mineralTaken);
            }

            // reproduction requirements (first pass: mineral level 1.0, health level 0.75)
            if (organism.Intention.Equals(Intention.Reproduce)
                && environment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0)
                && organism.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                // TODO: create the result of using the mineral during reproduction!  a child organism?!
                mineralTaken = availableMineral;
            }

            return mineralTaken;
        }

        private static double ProcessMineral(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var availableMineral = measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral);
            var mineralTaken = 0.0;

            if (availableMineral.Equals(0.0))
            {
                return mineralTaken;
            }

            if (organism.Intention.Equals(Intention.Mine))
            {
                var desiredMineral = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                mineralTaken = Math.Min(desiredMineral, availableMineral);
                organism.IncreaseLevel(OrganismMeasure.Inventory, mineralTaken);
            }

            // reproduction requirements (first pass: mineral level 1.0, health level 0.75)
            if (organism.Intention.Equals(Intention.Reproduce)
                && measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0)
                && organism.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                // TODO: create the result of using the mineral during reproduction!  a child organism?!
                mineralTaken = availableMineral;
            }

            return mineralTaken;
        }

        private static double ProcessHazards(Coordinate organismCoordinate, EcosystemData ecosystemData)
        {
            var organism = ecosystemData.GetOrganism(organismCoordinate);
            var environment = ecosystemData.GetEnvironment(organismCoordinate);

            var hazardousMeasurements = environment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            var obstructionCreated = 0.0;

            if (!organism.Intention.Equals(Intention.Build)
                || organism.GetLevel(OrganismMeasure.Inventory) < 1.0)
            {
                return obstructionCreated;
            }

            if (hazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                ecosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Inventory, 1.0);
                obstructionCreated = 1.0;
            }

            return obstructionCreated;
        }

        private static double ProcessHazards(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var hazardousMeasurements = measurableEnvironment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            var obstructionCreated = 0.0;

            if (!organism.Intention.Equals(Intention.Build)
                || organism.GetLevel(OrganismMeasure.Inventory) < 1.0)
            {
                return obstructionCreated;
            }

            if (hazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                organism.DecreaseLevel(OrganismMeasure.Inventory, 1.0);
                obstructionCreated = 1.0;
            }

            return obstructionCreated;
        }
    }
}
