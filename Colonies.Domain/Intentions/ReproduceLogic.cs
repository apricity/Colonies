namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class ReproduceLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory => Inventory.Spawn;
        public Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
                                                                         {
                                                                             { EnvironmentMeasure.Nutrient, 0 },
                                                                             { EnvironmentMeasure.Pheromone, 0 },
                                                                             { EnvironmentMeasure.Sound, 0 },
                                                                             { EnvironmentMeasure.Damp, 0 },
                                                                             { EnvironmentMeasure.Heat, 0 },
                                                                             { EnvironmentMeasure.Disease, 0 },
                                                                             { EnvironmentMeasure.Obstruction, 0 },
                                                                         };

        public bool CanInteractEnvironment(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Health) >= 0.995 && organismState.GetLevel(OrganismMeasure.Inventory).Equals(0.0);
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.CanInteractEnvironment(organismState) && this.EnvironmentHasFullMinerals(measurableEnvironment);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (!this.CanInteractEnvironment(measurableEnvironment, organismState))
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

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }

        private bool EnvironmentHasFullMinerals(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0);
        }
    }
}
