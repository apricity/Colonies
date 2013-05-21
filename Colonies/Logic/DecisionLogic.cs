namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public static class DecisionLogic
    {
        private static Random random = new Random();
        private static double baseWeighting = 1.0;

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
                // each measurement initially has a base weighting (the greater it is, the less effect the measurement level and bias have)
                // add further weighting according to strength of measurement with bias applied
                var weighting = baseWeighting + measurement.Value.Conditions.Sum(condition => condition.Level * condition.Bias);
                measurement.Value.SetWeighting(weighting);
            }
        }

        private static T ChooseRandomItem<T>(Dictionary<T, Measurement> weightedMeasuredItems)
        {
            var chosenItem = default(T);
            var totalWeight = weightedMeasuredItems.Values.Sum(measurement => measurement.Weighting);

            var randomNumber = random.NextDouble() * totalWeight;
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

        public static void SetRandomNumberGenerator(Random randomNumberGenerator)
        {
            random = randomNumberGenerator;
        }

        public static void SetBaseWeighting(double weighting)
        {
            baseWeighting = weighting;
        }
    }
}
