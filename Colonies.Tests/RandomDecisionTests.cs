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
        private List<string> items;

        [SetUp]
        public void SetupTest()
        {
            this.items = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };
        }

        [Test]
        public void EqualItems()
        {
            // all items will only have one measurement, and they will all be the same
            var measurement = new Measurement(new List<Condition> { new Condition(Measure.Pheromone, 1.0) });
            var itemMeasurements = this.items.ToDictionary(item => item, item => measurement);

            // there needs to be a bias for each measure (will be applied to all items)
            // but will have no effect when there is only one measure
            var measureBiases = new Dictionary<Measure, double> { { Measure.Pheromone, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);

            var chosenItems = new List<string>();
            
            // the numbers generated are distributed evenly based on the number of organisms
            // therefore there should be each organism should be chosen once by the decision logic
            var increment = 1.0 / this.items.Count;
            for (var nextDouble = 0.0; nextDouble < 1.0; nextDouble += increment)
            {
                mockRandom.SetNextDouble(nextDouble);
                chosenItems.Add(DecisionLogic.MakeDecision(itemMeasurements, measureBiases));
            }

            // expecting each organism to be chosen once, so expecting the same as the original list
            var expectedItems = this.items;
            var actualItems = chosenItems;
            Assert.That(actualItems, Is.EqualTo(expectedItems));
        }

        [Test]
        public void InequalItems()
        {
            // all items will only have one measurement but the levels will all be different
            // by an even spread based on how many items there are
            var itemMeasurements = new Dictionary<string, Measurement>();
            var measurementLevelChange = 1.0 / this.items.Count;
            for (var i = 0; i < this.items.Count; i++)
            {
                var measurementLevel = (i + 1) * measurementLevelChange;
                var measurement = new Measurement(new List<Condition> { new Condition(Measure.Pheromone, measurementLevel) });
                itemMeasurements.Add(this.items.ElementAt(i), measurement);
            }

            // there needs to be a bias for each measure (will be applied to all items)
            // but will have no effect when there is only one measure
            var measureBiases = new Dictionary<Measure, double> { { Measure.Pheromone, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            // and set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);
            DecisionLogic.SetBaseWeighting(0.0);

            var chosenItems = new List<string>();

            // the numbers generated need to reflect the range of measurement levels in the items
            // if there are 8 items...
            // - 8/8 pheromone -> 8 results
            // - 7/8 pheromone -> 7 results
            // - 6/8 pheromone -> 6 results
            // - ...
            // - 1/8 pheromone -> 1 result
            // total number of results: 1 + 2 + 3 + 4 + ... = n(n + 1)/2 [sum of integers from 1 - n]
            var numberOfResults = this.items.Count() * (this.items.Count() + 1) / 2;
            var increment = 1.0 / numberOfResults;
            for (var nextDouble = 0.0; nextDouble < 1.0; nextDouble += increment)
            {
                mockRandom.SetNextDouble(nextDouble);
                chosenItems.Add(DecisionLogic.MakeDecision(itemMeasurements, measureBiases));
            }

            // expecting each organism to be chosen a number of times proportional to their health
            var expectedItemCounts = new Dictionary<string, int>();
            for (var i = 0; i < this.items.Count; i++)
            {
                expectedItemCounts.Add(this.items.ElementAt(i), i + 1);
            }

            var actualItemCounts = new Dictionary<string, int>();
            foreach (var item in this.items)
            {
                var numberOfTimesChosen = chosenItems.Count(chosenItem => chosenItem.Equals(item));
                actualItemCounts.Add(item, numberOfTimesChosen);
            }

            Assert.That(actualItemCounts, Is.EqualTo(expectedItemCounts));
        }
    }
}


