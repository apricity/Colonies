namespace Wacton.Colonies.Domain.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Tovarisch.Randomness;

    public static class Decider
    {
        private static double baseWeighting = 1.0;

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
                // add further weighting according to strength of measurement with bias applied
                var measurementData = measurable.MeasurementData;
                var weighting = measurementData.Measurements.Sum(measurement => measurement.Level * biases[measurement.Measure]);

                weightedMeasurables.Add(measurable, weighting);
            }

            // shift weightings so they are all positive
            var minWeight = weightedMeasurables.Values.Min();
            var shiftedWeightedMeasurables = new Dictionary<TMeasurable, double>();
            foreach (var weightedMeasurable in weightedMeasurables)
            {
                // apply a base weighting per measurement data (the greater it is, the less effect the measurement level and bias have)
                var shiftedWeighting = weightedMeasurable.Value + baseWeighting;
                if (minWeight < 0)
                {
                    shiftedWeighting += Math.Abs(minWeight);
                }

                shiftedWeightedMeasurables.Add(weightedMeasurable.Key, shiftedWeighting);
            }

            return shiftedWeightedMeasurables;
        }

        private static TMeasurable ChooseRandomMeasurable<TMeasurable, TMeasure>(Dictionary<TMeasurable, double> weightedMeasurables)
            where TMeasurable : IMeasurable<TMeasure>
            where TMeasure : IMeasure
        {
            var chosenMeasurable = default(TMeasurable);
            var totalWeight = weightedMeasurables.Values.Sum(weight => weight);

            // this occurs when there is no base weighting, and no bias towards any present measures
            // but base weight of zero should probably only be used for testing
            if (totalWeight.Equals(0.0))
            {
                throw new InvalidOperationException("The total weight of the measurables is 0; no bias and no base weighting?");
                //return SelectOne(weightedMeasurables.Keys);
            }

            var randomNumber = RandomNumberGenerator.DoubleBetween(0, totalWeight);
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
