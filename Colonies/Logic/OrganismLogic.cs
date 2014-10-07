namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models;
    using Wacton.Colonies.Models.Interfaces;

    public static class OrganismLogic
    {
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

        private static void ConsumeInventoryNutrients(Organism organism)
        {
            if (!organism.Inventory.Measure.Equals(EnvironmentMeasure.Nutrient))
            {
                return;
            }

            var availableInventoryNutrient = organism.Inventory.Level;
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
            organism.IncreaseLevel(OrganismMeasure.Health, inventoryNutrientTaken);
            organism.Inventory.DecreaseLevel(inventoryNutrientTaken);
        }

        private static double ProcessNutrient(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var availableNutrient = measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient);
            var nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (organism.Intention.Equals(Intention.Harvest) && organism.Inventory.Measure.Equals(EnvironmentMeasure.Nutrient))
            {
                var desiredNutrient = 1 - organism.Inventory.Level;
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                organism.Inventory.IncreaseLevel(nutrientTaken);
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                organism.IncreaseLevel(OrganismMeasure.Health, nutrientTaken);
            }

            return nutrientTaken;
        }

        private static double ProcessMineral(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var availableMineral = measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral);
            var mineralTaken = 0.0;

            if (availableMineral.Equals(0.0))
            {
                return mineralTaken;
            }

            if (organism.Intention.Equals(Intention.Mine) && organism.Inventory.Measure.Equals(EnvironmentMeasure.Mineral))
            {
                var desiredMineral = 1 - organism.Inventory.Level;
                mineralTaken = Math.Min(desiredMineral, availableMineral);
                organism.Inventory.IncreaseLevel(mineralTaken);
            }

            // reproduction requirements (first pass: mineral level 1.0, health level 0.75)
            if (organism.Intention.Equals(Intention.Reproduce)
                && measurableEnvironment.MeasurementData.GetLevel(EnvironmentMeasure.Mineral) < 1.0
                && organism.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                // TODO: create the result of using the mineral during reproduction!  a child organism?!
                mineralTaken = availableMineral;
            }

            return mineralTaken;
        }

        private static double ProcessHazards(Organism organism, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var hazardousMeasurements = measurableEnvironment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            var obstructionCreated = 0.0;

            if (!organism.Intention.Equals(Intention.Build)
                || !organism.Inventory.Measure.Equals(EnvironmentMeasure.Mineral) 
                || organism.Inventory.Level < 1.0)
            {
                return obstructionCreated;
            }

            if (hazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                organism.Inventory.DecreaseLevel(1.0);
                obstructionCreated = 1.0;
            }

            return obstructionCreated;
        }
    }
}
