namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Interfaces;

    public static class DecisionLogic
    {
        private static double baseWeighting = 1.0;

        public static IMeasurable<T> MakeDecision<T>(IEnumerable<IMeasurable<T>> measurableItems, IBiased<T> biasProvider)
            where T : IMeasure
        {
            var weightedMeasuredItems = WeightMeasuredItems(measurableItems, biasProvider);
            var chosenItem = ChooseRandomItem(weightedMeasuredItems);

            if (chosenItem == null)
            {
                throw new NullReferenceException("An item has not been chosen");
            }

            return chosenItem;
        }

        public static T MakeDecision<T>(IEnumerable<T> items)
        {
            var chosenItem = default(T);
            var itemList = items.ToList();

            var itemChosen = false;
            var randomNumber = RandomNumberGenerator.RandomDouble(itemList.Count);
            foreach (var item in itemList)
            {
                if (randomNumber <= 1)
                {
                    chosenItem = item;
                    itemChosen = true;
                    break;
                }

                randomNumber -= 1;
            }

            if (!itemChosen)
            {
                throw new NullReferenceException("An item has not been chosen");
            }

            return chosenItem;
        }

        public static bool IsSuccessful(double successProbability)
        {
            if (successProbability < 0 || successProbability > 1)
            {
                throw new ArgumentOutOfRangeException("successProbability", successProbability, "Success probability must be between 0 - 1");
            }

            if (successProbability == 0.0)
            {
                return false;
            }

            var randomNumber = RandomNumberGenerator.RandomDouble(1);
            return randomNumber <= successProbability;
        }

        private static Dictionary<IMeasurable<T>, double> WeightMeasuredItems<T>(IEnumerable<IMeasurable<T>> measurableItems, IBiased<T> biasProvider) 
            where T : IMeasure
        {
            var weightedMeasurableItems = new Dictionary<IMeasurable<T>, double>();

            var biases = biasProvider.MeasureBiases;
            foreach (var measurableItem in measurableItems)
            {
                // TODO: easy for a bug to occur if bias does not contain a measure in the measurable items
                // each measurement initially has a base weighting (the greater it is, the less effect the measurement level and bias have)
                // add further weighting according to strength of measurement with bias applied
                var measurement = measurableItem.Measurement;
                var weighting = baseWeighting + measurement.Conditions.Sum(condition => condition.Level * biases[(T)condition.Measure]);

                weightedMeasurableItems.Add(measurableItem, weighting);
            }

            return weightedMeasurableItems;
        }

        private static IMeasurable<T> ChooseRandomItem<T>(Dictionary<IMeasurable<T>, double> weightedMeasurableItems) where T : IMeasure
        {
            var chosenItem = default(IMeasurable<T>);
            var totalWeight = weightedMeasurableItems.Values.Sum(weight => weight);

            var randomNumber = RandomNumberGenerator.RandomDouble(totalWeight);
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

        public static void SetBaseWeighting(double weighting)
        {
            baseWeighting = weighting;
        }
    }
}
