namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Interfaces;

    public static class DecisionLogic
    {
        private static double baseWeighting = 1.0;

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

        public static TMeasurable MakeDecision<TMeasurable, TMeasure>(IEnumerable<TMeasurable> measurables, IBiased<TMeasure> biasProvider)
            where TMeasurable : IMeasurable<TMeasure>
            where TMeasure : IMeasure
        {
            var weightedMeasurables = WeightMeasurables(measurables, biasProvider);
            var chosenMeasurable = ChooseRandomMeasurable<TMeasurable, TMeasure>(weightedMeasurables);

            if (chosenMeasurable == null)
            {
                throw new NullReferenceException("A measurable has not been chosen");
            }

            return chosenMeasurable;
        }

        private static Dictionary<TMeasurable, double> WeightMeasurables<TMeasurable, TMeasure>(IEnumerable<TMeasurable> measurables, IBiased<TMeasure> biasProvider)
            where TMeasurable : IMeasurable<TMeasure>
            where TMeasure : IMeasure
        {
            var weightedMeasurables = new Dictionary<TMeasurable, double>();

            var biases = biasProvider.MeasureBiases;
            foreach (var measurable in measurables)
            {
                // TODO: easy for a bug to occur if bias does not contain a measure in the measurables list
                // each measurement initially has a base weighting (the greater it is, the less effect the measurement level and bias have)
                // add further weighting according to strength of measurement with bias applied
                var measurementData = measurable.MeasurementData;
                var weighting = baseWeighting + measurementData.Measurements.Sum(measurement => measurement.Level * biases[(TMeasure)measurement.Measure]);

                weightedMeasurables.Add(measurable, weighting);
            }

            return weightedMeasurables;
        }

        private static TMeasurable ChooseRandomMeasurable<TMeasurable, TMeasure>(Dictionary<TMeasurable, double> weightedMeasurables)
            where TMeasurable : IMeasurable<TMeasure>
            where TMeasure : IMeasure
        {
            var chosenMeasurable = default(TMeasurable);
            var totalWeight = weightedMeasurables.Values.Sum(weight => weight);

            var randomNumber = RandomNumberGenerator.RandomDouble(totalWeight);
            foreach (var weightedMeasurable in weightedMeasurables)
            {
                if (randomNumber < weightedMeasurable.Value)
                {
                    chosenMeasurable = weightedMeasurable.Key;
                    break;
                }

                randomNumber -= weightedMeasurable.Value;
            }

            return chosenMeasurable;
        }

        public static void SetBaseWeighting(double weighting)
        {
            baseWeighting = weighting;
        }
    }
}
