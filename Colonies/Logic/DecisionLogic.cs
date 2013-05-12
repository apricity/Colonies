namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class DecisionLogic : IDecisionLogic
    {
        public List<Measurement> MakeDecision(List<List<Measurement>> measurementsCollection, Random random)
        {
            var weightedMeasurementsCollection = this.WeightMeasurementsCollection(measurementsCollection);
            var chosenMeasurements = this.ChooseRandomMeasurements(weightedMeasurementsCollection, random);
            if (chosenMeasurements == null)
            {
                throw new NullReferenceException("A set of measurements has not been chosen");
            }

            return chosenMeasurements;
        }

        private Dictionary<List<Measurement>, double> WeightMeasurementsCollection(List<List<Measurement>> measurementsCollection)
        {
            var weightedMeasurementsCollection = new Dictionary<List<Measurement>, double>();
            foreach (var measurements in measurementsCollection)
            {
                // each stimulus initially has a '1' rating
                // add further weighting according to strength of measurement
                var currentWeight = 1.0;

                foreach (var measurement in measurements)
                {
                    currentWeight += measurement.Level * measurement.Bias;
                }

                weightedMeasurementsCollection.Add(measurements, currentWeight);
            }

            return weightedMeasurementsCollection;
        }

        private List<Measurement> ChooseRandomMeasurements(Dictionary<List<Measurement>, double> weightedMeasurementsCollection, Random random)
        {
            List<Measurement> chosenMeasurements = null;
            var totalWeight = weightedMeasurementsCollection.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * totalWeight;
            foreach (var weightedMeasurements in weightedMeasurementsCollection)
            {
                if (randomNumber < weightedMeasurements.Value)
                {
                    chosenMeasurements = weightedMeasurements.Key;
                    break;
                }

                randomNumber -= weightedMeasurements.Value;
            }

            return chosenMeasurements;
        }
    }
}
