namespace Wacton.Colonies.DataTypes.Enums
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class HarvestLogic : IIntentionLogic
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
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.OrganismCanHarvest(organismState) && this.EnvironmentHasNutrients(measurableEnvironment);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (!this.CanInteractEnvironment(measurableEnvironment, organismState))
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

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }

        private bool OrganismCanHarvest(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Nutrient) && organismState.GetLevel(OrganismMeasure.Inventory) < 1.0;
        }

        private bool EnvironmentHasNutrients(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient) > 0.0;
        }
    }
}
