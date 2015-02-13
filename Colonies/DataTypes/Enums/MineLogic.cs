namespace Wacton.Colonies.DataTypes.Enums
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class MineLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory
        {
            get
            {
                return Inventory.Mineral;
            }
        }

        public Dictionary<EnvironmentMeasure, double> EnvironmentBias
        {
            get
            {
                return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Mineral, 25 },
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 },
                           { EnvironmentMeasure.Obstruction, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.OrganismCanMine(organismState) && this.EnvironmentHasMinerals(measurableEnvironment);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (!this.CanInteractEnvironment(measurableEnvironment, organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            var availableMineral = measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral);
            var desiredMineral = 1 - organismState.GetLevel(OrganismMeasure.Inventory);
            var mineralTaken = Math.Min(desiredMineral, availableMineral);

            organismAdjustments.Add(OrganismMeasure.Inventory, mineralTaken);
            environmentAdjustments.Add(EnvironmentMeasure.Mineral, -mineralTaken);

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

        private bool OrganismCanMine(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Mineral) && organismState.GetLevel(OrganismMeasure.Inventory) < 1.0;
        }

        private bool EnvironmentHasMinerals(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral) > 0.0;
        }
    }
}
