namespace Wacton.Colonies.Domain.Intentions
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class MineLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Mineral;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Mineral, 25 },
                { EnvironmentMeasure.Damp, -10 },
                { EnvironmentMeasure.Heat, -10 },
                { EnvironmentMeasure.Disease, -50 },
                { EnvironmentMeasure.Obstruction, -50 }
            };

        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismNeedsMinerals(organismState) 
                && EnvironmentHasMinerals(measurableEnvironment);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (!this.CanPerformAction(organismState, measurableEnvironment))
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

        private static bool OrganismNeedsMinerals(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Mineral) && organismState.GetLevel(OrganismMeasure.Inventory) < 1.0;
        }

        private static bool EnvironmentHasMinerals(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral) > 0.0;
        }
    }
}
