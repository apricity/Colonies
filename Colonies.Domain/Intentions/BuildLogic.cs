namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class BuildLogic : ActionIntentionLogic
    {
        public override Inventory AssociatedIntenvory => Inventory.Mineral;
        public override Dictionary<EnvironmentMeasure, double> EnvironmentBias => new Dictionary<EnvironmentMeasure, double>
            {
                { EnvironmentMeasure.Sound, 50 },
                { EnvironmentMeasure.Damp, 10 },
                { EnvironmentMeasure.Heat, 10 },
                { EnvironmentMeasure.Disease, 25 },
                { EnvironmentMeasure.Obstruction, -50 }
            };

        public override bool CanPerformAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismHasMinerals(organismState) 
                && EnvironmentHasHazard(measurableEnvironment) 
                && !EnvironmentHasObstruction(measurableEnvironment);
        }

        public override IntentionAdjustments EffectsOfAction(IOrganismState organismState, IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (!this.CanPerformAction(organismState, measurableEnvironment))
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
        private static bool OrganismHasMinerals(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Mineral) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(1.0);
        }

        private static bool EnvironmentHasHazard(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            var hazardousMeasurements = measurableEnvironment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();
            return hazardousMeasurements.Any(measurement => measurement.Level > 0.0);
        }

        private static bool EnvironmentHasObstruction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return measurableEnvironment.GetLevel(EnvironmentMeasure.Obstruction) > 0.0;
        }
    }
}
