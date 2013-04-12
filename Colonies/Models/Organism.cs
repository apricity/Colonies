namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public sealed class Organism
    {
        private string Name { get; set; }
        public Color Color { get; set; }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        // TODO: this should be a method that calculates an INTENTION based on conditions (does not necessarily choose a condition to move to)
        public HabitatCondition TakeTurn(IEnumerable<HabitatCondition> possibleHabitatConditions, Random random)
        {
            HabitatCondition selectedHabitat = null;

            var weightedHabitatConditions = WeightHabitatConditions(possibleHabitatConditions);
            var sumOfWeights = weightedHabitatConditions.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * sumOfWeights;
            foreach (var weightedHabitatCondition in weightedHabitatConditions)
            {
                if (randomNumber < weightedHabitatCondition.Value)
                {
                    selectedHabitat = weightedHabitatCondition.Key;
                    break;
                }

                randomNumber -= weightedHabitatCondition.Value;
            }

            if (selectedHabitat == null)
            {
                throw new NullReferenceException("A habitat has not been selected");
            }

            return selectedHabitat;
        }

        private static Dictionary<HabitatCondition, double> WeightHabitatConditions(IEnumerable<HabitatCondition> possibleHabitatConditions)
        {
            var weightedHabitatConditions = new Dictionary<HabitatCondition, double>();
            foreach (var habitatCondition in possibleHabitatConditions)
            {
                // each habitat initially has a '1' rating
                // add further weighting according to strength of condition
                var currentWeight = 1.0;
                currentWeight += 10 * habitatCondition.Strength;

                weightedHabitatConditions.Add(habitatCondition, currentWeight);
            }

            return weightedHabitatConditions;
        }

        public override string ToString()
        {
            return string.Format("{0} <{1}>", this.Name, this.Color);
        }
    }
}