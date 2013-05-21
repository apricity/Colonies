namespace Colonies.Tests
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies;
    using Colonies.Logic;
    using Colonies.Models;
    using Colonies.Tests.Mocks;

    using NUnit.Framework;

    [TestFixture]
    public class RandomDecisionTests
    {
        private Dictionary<string, Organism> organisms;

        [SetUp]
        public void SetupTest()
        {
            var organismIdentifiers = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };

            this.organisms = new Dictionary<string, Organism>();
            foreach (var organismIdentifier in organismIdentifiers)
            {
                this.organisms.Add(organismIdentifier, new Organism(organismIdentifier, Color.Black, false));
            }
        }

        [Test]
        public void EqualOrganisms()
        {
            // all organisms will have the same measurements by default
            // (currently, health = 1.0)
            var organismMeasurements = this.organisms.Values.ToDictionary(
                organism => organism,
                organism => organism.GetMeasurement());

            // there needs to be a bias for each organism measure, and has to be the same for all organisms
            // (this will have no effect when there is only one measure)
            var measureBiases = new Dictionary<Measure, double> { { Measure.Health, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);

            var chosenOrganisms = new List<Organism>();
            
            // the numbers generated are distributed evenly based on the number of organisms
            // therefore there should be each organism should be chosen once by the decision logic
            var increment = 1.0 / this.organisms.Count;
            for (var nextDouble = 0.0; nextDouble < 1.0; nextDouble += increment)
            {
                mockRandom.SetNextDouble(nextDouble);
                chosenOrganisms.Add(DecisionLogic.MakeDecision(organismMeasurements, measureBiases));
            }

            // expecting each organism to be chosen once, so expecting the same as the original list
            var expectedOrganisms = this.organisms.Values.ToList();
            var actualOrganisms = chosenOrganisms;
            Assert.That(actualOrganisms, Is.EqualTo(expectedOrganisms));
        }

        [Test]
        public void InequalOrganisms()
        {
            // all organisms will have the same measurements by default
            // (currently, health = 1.0)
            var organismMeasurements = this.organisms.Values.ToDictionary(
                organism => organism,
                organism => organism.GetMeasurement());

            // set organism health levels to change the frequency with which they are chosen
            var decrement = 1.0 / this.organisms.Count;
            for (var i = 0; i < this.organisms.Count; i++)
            {
                var healthDecrement = i * decrement;
                organismMeasurements.ElementAt(i).Key.DecreaseHealth(healthDecrement);
            }

            // there needs to be a bias for each organism measure, and has to be the same for all organisms
            // (this will have no effect when there is only one measure)
            var measureBiases = new Dictionary<Measure, double> { { Measure.Health, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            // and set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);
            DecisionLogic.SetBaseWeighting(0.0);

            var chosenOrganisms = new List<Organism>();

            // the numbers generated need to reflect the range of health levels in the organisms
            // if there are 8 organisms...
            // - 8/8 health -> 8 results, 7/8 health -> 7 results, 6/8 health -> 6 results, ..., 1/8 health -> 1 result
            // total number of results: 1 + 2 + 3 + 4 + ... = n(n + 1)/2 [sum of integers from 1 - n]
            var numberOfResults = this.organisms.Count() * (this.organisms.Count() + 1) / 2;
            var increment = 1.0 / numberOfResults;
            for (var nextDouble = 0.0; nextDouble < 1.0; nextDouble += increment)
            {
                mockRandom.SetNextDouble(nextDouble);
                chosenOrganisms.Add(DecisionLogic.MakeDecision(organismMeasurements, measureBiases));
            }

            // expecting each organism to be chosen a number of times proportional to their health
            var expectedOrganismCounts = new Dictionary<Organism, int>();
            for (var i = 0; i < this.organisms.Count; i++)
            {
                expectedOrganismCounts.Add(this.organisms.ElementAt(i).Value, this.organisms.Count - i);
            }

            var actualOrganismCounts = new Dictionary<Organism, int>();
            foreach (var organism in this.organisms.Values)
            {
                var numberOfTimesChosen = chosenOrganisms.Count(chosenOrganism => chosenOrganism.Equals(organism));
                actualOrganismCounts.Add(organism, numberOfTimesChosen);
            }

            Assert.That(actualOrganismCounts, Is.EqualTo(expectedOrganismCounts));
        }
    }
}
