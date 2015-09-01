namespace Wacton.Colonies.Domain.Intentions
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class EatLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory
        {
            get
            {
                return Inventory.Nutrient;
            }
        }

        public Dictionary<EnvironmentMeasure, double> EnvironmentBias
        {
            get
            {
                return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Nutrient, 10 },
                           { EnvironmentMeasure.Pheromone, 10 },
                           { EnvironmentMeasure.Sound, 10 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IOrganismState organismState)
        {
            return organismState.GetLevel(OrganismMeasure.Health) < 1.0;
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.CanInteractEnvironment(organismState) && 
                (this.OrganismHasNutrients(organismState) || this.EnvironmentHasNutrients(measurableEnvironment));
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (!this.CanInteractEnvironment(measurableEnvironment, organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            // primary choice is to eat from inventory, secondary is to eat from environment
            if (this.OrganismHasNutrients(organismState))
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

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }

        private bool OrganismHasNutrients(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Nutrient) && organismState.GetLevel(OrganismMeasure.Inventory) > 0.0;
        }

        private bool EnvironmentHasNutrients(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient) > 0.0;
        }
    }
}
