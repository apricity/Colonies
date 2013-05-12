namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class DecisionLogic : IDecisionLogic
    {
        public List<Stimulus> MakeDecision(List<List<Stimulus>> stimuli, Random random)
        {
            var weightedStimuli = WeightStimuli(stimuli);
            var chosenStimulus = ChooseRandomStimulus(weightedStimuli, random);
            if (chosenStimulus == null)
            {
                throw new NullReferenceException("A stimulus has not been chosen");
            }

            return chosenStimulus;
        }

        private Dictionary<List<Stimulus>, double> WeightStimuli(List<List<Stimulus>> stimuli)
        {
            var weightedStimuli = new Dictionary<List<Stimulus>, double>();
            foreach (var stimulusSet in stimuli)
            {
                // each stimulus initially has a '1' rating
                // add further weighting according to strength of measurement
                var currentWeight = 1.0;

                foreach (var stimulus in stimulusSet)
                {
                    currentWeight += stimulus.Level * stimulus.Bias;
                }

                weightedStimuli.Add(stimulusSet, currentWeight);
            }

            return weightedStimuli;
        }

        private List<Stimulus> ChooseRandomStimulus(Dictionary<List<Stimulus>, double> weightedStimuli, Random random)
        {
            List<Stimulus> chosenStimulus = null;
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
