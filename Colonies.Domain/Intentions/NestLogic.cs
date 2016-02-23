namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class NestLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Spawn;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Mineral, 25 },
                { EnvironmentMeasure.Damp, -10 },
                { EnvironmentMeasure.Heat, -10 },
                { EnvironmentMeasure.Disease, -50 }
            };

        // TODO: currently useless, consider generating spawn as minerals are consumed
        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismNeedsSpawn(organismState);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return new IntentionAdjustments();
        }

        private static bool OrganismNeedsSpawn(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(0.0);
        }
    }
}
