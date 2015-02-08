namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
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

            // organism inventory of "spawn" is filled, ready for the organism to place it
            if (organism.IsReproductive)
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

            if (environment.IsHarmful)
            {
                foreach (var harmfulMeasure in environment.HarmfulMeasures)
                {
                    this.harmfulEnvironmentActions[harmfulMeasure].Invoke(organism);
                }
            }

            if (!organism.Intention.Equals(Intention.Build) || organism.GetLevel(OrganismMeasure.Inventory) < 1.0)
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