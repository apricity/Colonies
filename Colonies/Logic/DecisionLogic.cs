namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public static class DecisionLogic
    {
        private static readonly Random Random = new Random();

        public static T MakeDecision<T>(Dictionary<T, Measurement> measuredItems, Dictionary<Measure, double> biases)
        {
            ApplyMeasurementBias(measuredItems, biases);
            WeightMeasuredItems(measuredItems);
            var chosenItem = ChooseRandomItem(measuredItems);
            if (chosenItem == null)
            {
                throw new NullReferenceException("A measurement has not been chosen");
            }

            return chosenItem;
        }

        private static void ApplyMeasurementBias<T>(Dictionary<T, Measurement> measuredItems, Dictionary<Measure, double> biases)
        {
            foreach (var measurement in measuredItems.Values)
            {
                foreach (var condition in measurement.Conditions)
                {
                    condition.SetBias(biases[condition.Measure]);
                }
            }
        }

        private static void WeightMeasuredItems<T>(Dictionary<T, Measurement> measuredItems)
        {
            foreach (var measurement in measuredItems)
            {
                // each measurement initially has a '1' rating
                // add further weighting according to strength of measurement with bias applied
                var weighting = 1.0 + measurement.Value.Conditions.Sum(condition => condition.Level * condition.Bias);
                measurement.Value.SetWeighting(weighting);
            }
        }

        private static T ChooseRandomItem<T>(Dictionary<T, Measurement> weightedMeasuredItems)
        {
            T chosenItem = default(T);
            var totalWeight = weightedMeasuredItems.Values.Sum(measurement => measurement.Weighting);

            var randomNumber = Random.NextDouble() * totalWeight;
            foreach (var weightedMeasuredItem in weightedMeasuredItems)
            {
                if (randomNumber < weightedMeasuredItem.Value.Weighting)
                {
                    chosenItem = weightedMeasuredItem.Key;
                    break;
                }

                randomNumber -= weightedMeasuredItem.Value.Weighting;
            }

            return chosenItem;
        }
    }
}
