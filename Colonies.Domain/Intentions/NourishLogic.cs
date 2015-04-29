namespace Wacton.Colonies.Domain.Intentions
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organism;

    public class NourishLogic : IIntentionLogic
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
                           { EnvironmentMeasure.Sound, 25 },
                           { EnvironmentMeasure.Pheromone, -25 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return new IntentionAdjustments();
        }

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return this.OrganismHasNutrients(organismState);
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            if (!this.CanInteractOrganism(organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            var availableNutrient = organismState.GetLevel(OrganismMeasure.Inventory);
            var desiredNutrient = 1 - otherOrganismState.GetLevel(OrganismMeasure.Health);
            var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);

            organismAdjustments.Add(OrganismMeasure.Inventory, -nutrientTaken);

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private bool OrganismHasNutrients(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Nutrient) && organismState.GetLevel(OrganismMeasure.Inventory) > 0.0;
        }
    }
}
