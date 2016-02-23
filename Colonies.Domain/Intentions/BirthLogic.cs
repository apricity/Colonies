namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class BirthLogic : InteractionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Spawn;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Damp, -10 },
                { EnvironmentMeasure.Heat, -10 },
                { EnvironmentMeasure.Disease, -50 }
            };

        public override bool CanPerformInteraction(IOrganismState organismState)
        {
            return OrganismHasSpawn(organismState);
        }

        public override IntentionAdjustments EffectsOfInteraction(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            if (!this.CanPerformInteraction(organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();
            organismAdjustments.Add(OrganismMeasure.Inventory, -1.0);

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private static bool OrganismHasSpawn(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(1.0);
        }
    }
}
