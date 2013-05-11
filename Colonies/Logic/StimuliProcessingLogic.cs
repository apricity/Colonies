namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class StimuliProcessingLogic : IStimuliProcessingLogic
    {
        public Stimulus ProcessStimuli(IEnumerable<Stimulus> stimuli, double pheromoneWeighting, Random random)
        {
            var weightedStimuli = WeightStimuli(stimuli, pheromoneWeighting);

            var chosenStimulus = ChooseRandomStimulus(weightedStimuli, random);
            if (chosenStimulus == null)
            {
                throw new NullReferenceException("A stimulus has not been chosen");
            }

            return chosenStimulus;
        }

        private static Dictionary<Stimulus, double> WeightStimuli(IEnumerable<Stimulus> stimuli, double pheromoneWeighting)
        {
            var weightedStimuli = new Dictionary<Stimulus, double>();
            foreach (var stimulus in stimuli)
            {
                // each stimulus initially has a '1' rating
                // add further weighting according to strength of measurement
                var currentWeight = 1.0;
                currentWeight += stimulus.PheromoneLevel * pheromoneWeighting;

                weightedStimuli.Add(stimulus, currentWeight);
            }

            return weightedStimuli;
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
    }
}
