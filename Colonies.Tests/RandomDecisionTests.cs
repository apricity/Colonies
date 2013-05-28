﻿namespace Colonies.Tests
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
        private List<TestItem> items;

        [SetUp]
        public void SetupTest()
        {
            this.items = new List<TestItem>();

            var itemIdentifiers = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };
            foreach (var itemIdentifier in itemIdentifiers)
            {
                this.items.Add(new TestItem(itemIdentifier));
            }
        }

        [Test]
        public void EqualItems()
        {
            // there needs to be a bias for each measure (will be applied to all items)
            // but will have no effect when there is only one measure
            var measureBiases = new Dictionary<Measure, double> { { Measure.Pheromone, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);

            var chosenItems = new List<TestItem>();
            
            // the numbers generated are distributed evenly based on the number of organisms
            // therefore there should be each organism should be chosen once by the decision logic
            var increment = 1.0 / this.items.Count;
            for (var nextDouble = 0.0; nextDouble < 1.0; nextDouble += increment)
            {
                mockRandom.SetNextDouble(nextDouble);
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, measureBiases));
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
            var measurementLevelChange = 1.0 / this.items.Count;
            for (var i = 0; i < this.items.Count; i++)
            {
                var measurementLevel = (i + 1) * measurementLevelChange;
                this.items[i].SetPheromoneLevel(measurementLevel);
            }

            // there needs to be a bias for each measure (will be applied to all items)
            // but will have no effect when there is only one measure
            var measureBiases = new Dictionary<Measure, double> { { Measure.Pheromone, 1.0 } };

            // override the random number generator used by the decision logic so it can be manipulated
            // and set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            var mockRandom = new MockRandom();
            DecisionLogic.SetRandomNumberGenerator(mockRandom);
            DecisionLogic.SetBaseWeighting(0.0);

            var chosenItems = new List<TestItem>();

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
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, measureBiases));
            }

            // expecting each organism to be chosen a number of times proportional to their health
            var expectedItemCounts = new Dictionary<TestItem, int>();
            for (var i = 0; i < this.items.Count; i++)
            {
                expectedItemCounts.Add(this.items.ElementAt(i), i + 1);
            }

            var actualItemCounts = new Dictionary<TestItem, int>();
            foreach (var item in this.items)
            {
                var numberOfTimesChosen = chosenItems.Count(chosenItem => chosenItem.Equals(item));
                actualItemCounts.Add(item, numberOfTimesChosen);
            }

            Assert.That(actualItemCounts, Is.EqualTo(expectedItemCounts));
        }

        private class TestItem : IMeasurable
        {
            private readonly string identifier;
            private readonly Condition pheromoneCondition;
            private readonly Condition healthCondition;

            public TestItem(string identifier)
            {
                this.identifier = identifier;
                this.pheromoneCondition = new Condition(Measure.Pheromone, 1.0);
                this.healthCondition = new Condition(Measure.Health, 1.0);
            }

            public Measurement GetMeasurement()
            {
                // TODO: include a second condition and write tests to use both of them
                return new Measurement(new List<Condition> { this.pheromoneCondition });
            }

            public void SetPheromoneLevel(double pheromoneLevel)
            {
                this.pheromoneCondition.SetLevel(pheromoneLevel);
            }

            public override string ToString()
            {
                return this.identifier;
            }
        }
    }
}


