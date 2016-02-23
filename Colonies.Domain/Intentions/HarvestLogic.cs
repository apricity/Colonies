namespace Wacton.Colonies.Domain.Intentions
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class HarvestLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Nutrient;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Nutrient, 10 },
                { EnvironmentMeasure.Pheromone, 10 },
                { EnvironmentMeasure.Damp, -10 },
                { EnvironmentMeasure.Heat, -10 },
                { EnvironmentMeasure.Disease, -50 }
            };

        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismNeedsNutrients(organismState) 
                && EnvironmentHasNutrients(measurableEnvironment);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (!this.CanPerformAction(organismState, measurableEnvironment))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            var availableNutrient = measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient);
            var desiredNutrient = 1 - organismState.GetLevel(OrganismMeasure.Inventory);
            var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);

            organismAdjustments.Add(OrganismMeasure.Inventory, nutrientTaken);
            environmentAdjustments.Add(EnvironmentMeasure.Nutrient, -nutrientTaken);

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private static bool OrganismNeedsNutrients(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Nutrient) && organismState.GetLevel(OrganismMeasure.Inventory) < 1.0;
        }

        private static bool EnvironmentHasNutrients(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient) > 0.0;
        }
    }
}
