namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class NestLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory
        {
            get
            {
                return Inventory.Spawn;
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
                           { EnvironmentMeasure.Disease, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(0.0);
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.CanInteractEnvironment(organismState);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return new IntentionAdjustments();
        }

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }

        private bool EnvironmentHasMinerals(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral) > 0.0;
        }
    }
}
