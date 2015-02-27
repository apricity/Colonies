namespace Wacton.Colonies.Intentions
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Measures;
    using Wacton.Colonies.Organism;

    public class BuildLogic : IIntentionLogic
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
                           { EnvironmentMeasure.Sound, 50 },
                           { EnvironmentMeasure.Damp, 10 },
                           { EnvironmentMeasure.Heat, 10 },
                           { EnvironmentMeasure.Disease, 25 },
                           { EnvironmentMeasure.Obstruction, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return this.OrganismCanBuild(organismState) 
                && this.EnvironmentHasHazard(measurableEnvironment) && !this.EnvironmentHasObstruction(measurableEnvironment);
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            if (!this.CanInteractEnvironment(measurableEnvironment, organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();

            var mineralTaken = organismState.GetLevel(OrganismMeasure.Inventory);
            organismAdjustments.Add(OrganismMeasure.Inventory, -mineralTaken);
            environmentAdjustments.Add(EnvironmentMeasure.Obstruction, mineralTaken);

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

        private bool OrganismCanBuild(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Mineral) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(1.0);
        }

        private bool EnvironmentHasHazard(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var hazardousMeasurements = measurableEnvironment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            return hazardousMeasurements.Any(measurement => measurement.Level > 0.0);
        }

        private bool EnvironmentHasObstruction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Obstruction) > 0.0;
        }
    }
}
