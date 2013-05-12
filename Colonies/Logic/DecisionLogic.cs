namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class DecisionLogic : IDecisionLogic
    {
        public Measurement MakeDecision(List<Measurement> measurements, Random random)
        {
            var weightedMeasurements = this.WeightMeasurements(measurements);
            var chosenMeasurement = this.ChooseRandomMeasurement(weightedMeasurements, random);
            if (chosenMeasurement == null)
            {
                throw new NullReferenceException("A measurement has not been chosen");
            }

            return chosenMeasurement;
        }

        private Dictionary<Measurement, double> WeightMeasurements(List<Measurement> measurements)
        {
            var weightedMeasurements = new Dictionary<Measurement, double>();
            foreach (var measurement in measurements)
            {
                // each measurement initially has a '1' rating
                // add further weighting according to strength of measurement with bias applied
                var currentWeighting = 1.0 + measurement.Conditions.Sum(condition => condition.Level * condition.Bias);
                weightedMeasurements.Add(measurement, currentWeighting);
            }

            return weightedMeasurements;
        }

        private Measurement ChooseRandomMeasurement(Dictionary<Measurement, double> weightedMeasurements, Random random)
        {
            Measurement chosenMeasurement = null;
            var totalWeight = weightedMeasurements.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * totalWeight;
            foreach (var weightedMeasurement in weightedMeasurements)
            {
                if (randomNumber < weightedMeasurement.Value)
                {
                    chosenMeasurement = weightedMeasurement.Key;
                    break;
                }

                randomNumber -= weightedMeasurement.Value;
            }

            return chosenMeasurement;
        }
    }
}
