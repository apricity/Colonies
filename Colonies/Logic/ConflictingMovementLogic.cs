namespace Colonies.Logic
{
    using System;

    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class ConflictingMovementLogic : IConflictingMovementLogic
    {
        public Organism DecideOrganism(IEnumerable<Organism> organisms, double healthWeighting, Random random)
        {
            var weightedOrganisms = WeightOrganisms(organisms, healthWeighting);

            var chosenStimulus = ChooseRandomOrganism(weightedOrganisms, random);
            if (chosenStimulus == null)
            {
                throw new NullReferenceException("A stimulus has not been chosen");
            }

            return chosenStimulus;
        }

        private static Dictionary<Organism, double> WeightOrganisms(IEnumerable<Organism> organisms, double healthWeighting)
        {
            var weightedOrganisms = new Dictionary<Organism, double>();
            foreach (var organism in organisms)
            {
                // each organism initially has a '1' rating
                // add further weighting according to strength of measurement
                var currentWeight = 1.0;
                currentWeight += organism.Health * healthWeighting;

                weightedOrganisms.Add(organism, currentWeight);
            }

            return weightedOrganisms;
        }

        private static Organism ChooseRandomOrganism(Dictionary<Organism, double> weightedOrganisms, Random random)
        {
            Organism chosenOrganism = null;
            var totalWeight = weightedOrganisms.Values.Sum(weight => weight);

            var randomNumber = random.NextDouble() * totalWeight;
            foreach (var weightedStimulus in weightedOrganisms)
            {
                if (randomNumber < weightedStimulus.Value)
                {
                    chosenOrganism = weightedStimulus.Key;
                    break;
                }

                randomNumber -= weightedStimulus.Value;
            }

            return chosenOrganism;
        }
    }
}
