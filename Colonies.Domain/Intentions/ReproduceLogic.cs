namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class ReproduceLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Spawn;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Nutrient, 0 },
                { EnvironmentMeasure.Pheromone, 0 },
                { EnvironmentMeasure.Sound, 0 },
                { EnvironmentMeasure.Damp, 0 },
                { EnvironmentMeasure.Heat, 0 },
                { EnvironmentMeasure.Disease, 0 },
                { EnvironmentMeasure.Obstruction, 0 },
            };

        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismAwaitingSpawn(organismState) 
                && OrganismIsHealthy(organismState)
                && EnvironmentHasFullMinerals(measurableEnvironment);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (!this.CanPerformAction(organismState, measurableEnvironment))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            var mineralTaken = measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral);
            organismAdjustments.Add(OrganismMeasure.Inventory, mineralTaken);
            environmentAdjustments.Add(EnvironmentMeasure.Mineral, -mineralTaken);

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private static bool OrganismAwaitingSpawn(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(0.0);
        }

        private static bool OrganismIsHealthy(IOrganismState organismState)
        {
            return organismState.GetLevel(OrganismMeasure.Health) >= 0.995;
        }

        private static bool EnvironmentHasFullMinerals(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0);
        }
    }
}
