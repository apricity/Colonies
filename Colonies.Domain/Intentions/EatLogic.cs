namespace Wacton.Colonies.Domain.Intentions
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class EatLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Nutrient;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Nutrient, 10 },
                { EnvironmentMeasure.Pheromone, 10 },
                { EnvironmentMeasure.Sound, 10 },
                { EnvironmentMeasure.Damp, -10 },
                { EnvironmentMeasure.Heat, -10 },
                { EnvironmentMeasure.Disease, -50 }
            };

        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismNeedsHealth(organismState)
                && NutrientsAreAvailable(organismState, measurableEnvironment);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (!this.CanPerformAction(organismState, measurableEnvironment))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            // primary choice is to eat from inventory, secondary is to eat from environment
            if (OrganismHasNutrients(organismState))
            {
                var availableInventoryNutrient = organismState.GetLevel(OrganismMeasure.Inventory);
                var desiredInventoryNutrient = 1 - organismState.GetLevel(OrganismMeasure.Health);
                var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
                
                organismAdjustments.Add(OrganismMeasure.Health, inventoryNutrientTaken);
                organismAdjustments.Add(OrganismMeasure.Inventory, -inventoryNutrientTaken);
            }
            else
            {
                var availableEnvironmentNutrient = measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient);
                var desiredEnvironmentNutrient = 1 - organismState.GetLevel(OrganismMeasure.Health);
                var environmentNutrientTaken = Math.Min(desiredEnvironmentNutrient, availableEnvironmentNutrient);

                organismAdjustments.Add(OrganismMeasure.Health, environmentNutrientTaken);
                environmentAdjustments.Add(EnvironmentMeasure.Nutrient, -environmentNutrientTaken);
            }

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private static bool OrganismNeedsHealth(IOrganismState organismState)
        {
            return organismState.GetLevel(OrganismMeasure.Health) < 1.0;
        }

        private static bool NutrientsAreAvailable(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismHasNutrients(organismState) 
                || EnvironmentHasNutrients(measurableEnvironment);
        }

        private static bool OrganismHasNutrients(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Nutrient) && organismState.GetLevel(OrganismMeasure.Inventory) > 0.0;
        }

        private static bool EnvironmentHasNutrients(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient) > 0.0;
        }
    }
}
