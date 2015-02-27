namespace Wacton.Colonies.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Measures;
    using Wacton.Colonies.Organism;

    public class NoLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory
        {
            get
            {
                return null;
            }
        }

        public Dictionary<EnvironmentMeasure, double> EnvironmentBias
        {
            get
            {
                return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Nutrient, 0 },
                           { EnvironmentMeasure.Pheromone, 0 },
                           { EnvironmentMeasure.Sound, 0 },
                           { EnvironmentMeasure.Damp, 0 },
                           { EnvironmentMeasure.Heat, 0 },
                           { EnvironmentMeasure.Disease, 0 },
                           { EnvironmentMeasure.Obstruction, 0 },
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
            return false;
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            return new IntentionAdjustments();
        }
    }
}
