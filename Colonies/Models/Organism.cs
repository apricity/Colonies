namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public sealed class Organism
    {
        private const int PheromoneMultiplier = 10;

        public string Name { get; private set; }
        public Color Color { get; private set; }
        public bool IsDepositingPheromones { get; private set; }

        public Organism(string name, Color color, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;
            this.IsDepositingPheromones = isDepostingPheromones;
        }

        // TODO: this should be a method that calculates an INTENTION based on conditions (does not necessarily choose a condition to move to)
        public Stimulus ProcessStimuli(IEnumerable<Stimulus> stimuli, Random random)
        {
            var weightedStimuli = WeightStimuli(stimuli);

            var chosenStimulus = ChooseRandomStimulus(weightedStimuli, random);
            if (chosenStimulus == null)
            {
                throw new NullReferenceException("A habitat has not been selected");
            }

            return chosenStimulus;
        }

        private static Stimulus ChooseRandomStimulus(Dictionary<Stimulus, double> weightedStimuli, Random random)
        {
            Stimulus chosenStimulus = null;
            var totalWeight = weightedStimuli.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * totalWeight;
            foreach (var weightedStimulus in weightedStimuli)
            {
                if (randomNumber < weightedStimulus.Value)
                {
                    chosenStimulus = weightedStimulus.Key;
                    break;
                }

                randomNumber -= weightedStimulus.Value;
            }

            return chosenStimulus;
        }

        private static Dictionary<Stimulus, double> WeightStimuli(IEnumerable<Stimulus> habitatStates)
        {
            var weightedHabitatStates = new Dictionary<Stimulus, double>();
            foreach (var habitatState in habitatStates)
            {
                // each habitat initially has a '1' rating
                // add further weighting according to strength of condition
                var currentWeight = 1.0;
                currentWeight += habitatState.PheromoneLevel * PheromoneMultiplier;

                weightedHabitatStates.Add(habitatState, currentWeight);
            }

            return weightedHabitatStates;
        }

        public override string ToString()
        {
            return string.Format("{0} <{1}>", this.Name, this.Color);
        }
    }
}