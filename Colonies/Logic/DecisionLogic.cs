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

        public static T MakeDecision<T>(List<T> measurableItems, IBiased biasProvider) where T : IMeasurable
        {
            var weightedMeasuredItems = WeightMeasuredItems(measurableItems, biasProvider);
            var chosenItem = ChooseRandomItem(weightedMeasuredItems);

            if (chosenItem == null)
            {
                throw new NullReferenceException("A measurement has not been chosen");
            }

            return chosenItem;
        }

        private static Dictionary<T, double> WeightMeasuredItems<T>(List<T> measurableItems, IBiased biasProvider) where T : IMeasurable
        {
            var weightedMeasurableItems = new Dictionary<T, double>();

            var biases = biasProvider.GetMeasureBiases();
            foreach (var measurableItem in measurableItems)
            {
                // TODO: easy for a bug to occur if bias does not contain a measure in the measurable items
                // each measurement initially has a base weighting (the greater it is, the less effect the measurement level and bias have)
                // add further weighting according to strength of measurement with bias applied
                var measurement = measurableItem.GetMeasurement();
                var weighting = baseWeighting + measurement.Conditions.Sum(condition => condition.Level * biases[condition.Measure]);

                weightedMeasurableItems.Add(measurableItem, weighting);
            }

            return weightedMeasurableItems;
        }

        private static T ChooseRandomItem<T>(Dictionary<T, double> weightedMeasurableItems) where T : IMeasurable
        {
            var chosenItem = default(T);
            var totalWeight = weightedMeasurableItems.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * totalWeight;
            foreach (var weightedMeasuredItem in weightedMeasurableItems)
            {
                if (randomNumber < weightedMeasuredItem.Value)
                {
                    chosenItem = weightedMeasuredItem.Key;
                    break;
                }

                randomNumber -= weightedMeasuredItem.Value;
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
