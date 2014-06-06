namespace Wacton.Colonies.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;
    using Wacton.Colonies.Utilities;

    [TestFixture]
    public class RandomDecisionTests
    {
        private List<TestMeasurableItem> items;

        [SetUp]
        public void SetupTest()
        {
            this.items = new List<TestMeasurableItem>();

            var itemIdentifiers = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };
            foreach (var itemIdentifier in itemIdentifiers)
            {
                this.items.Add(new TestMeasurableItem(itemIdentifier));
            }
        }

        [Test]
        public void EqualItemsSingleMeasure()
        {
            var biasedItem = new TestBiasedItem(1.0, 0);
            var chosenItems = new List<TestMeasurableItem>();

            // the numbers generated are distributed evenly based on the number of organisms
            // therefore there should be each organism should be chosen once by the decision logic
            var nextDouble = 0.0;
            for (var i = 0; i < this.items.Count; i++)
            {
                RandomNumberGenerator.OverrideNextDouble = nextDouble;
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, biasedItem));
                nextDouble += 1.0 / this.items.Count;
            }

            // expecting each item to be chosen once, so expecting the same as the original list
            var expectedItems = this.items;
            var actualItems = chosenItems;
            Assert.That(actualItems, Is.EqualTo(expectedItems));
        }

        [Test]
        public void EqualItemsMultipleMeasures()
        {
            // bias in favour of health 2x more than of pheromone
            var biasedItem = new TestBiasedItem(1.0, 2.0);
            var chosenItems = new List<TestMeasurableItem>();

            // the numbers generated are distributed evenly based on the number of organisms
            // therefore there should be each organism should be chosen once by the decision logic
            var nextDouble = 0.0;
            for (var i = 0; i < this.items.Count; i++)
            {
                RandomNumberGenerator.OverrideNextDouble = nextDouble;
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, biasedItem));
                nextDouble += 1.0 / this.items.Count;
            }

            // all items are equal, so the bias should not affect the frequency with which they are selected
            // expecting each item to be chosen once, so expecting the same as the original list
            var expectedItems = this.items;
            var actualItems = chosenItems;
            Assert.That(actualItems, Is.EqualTo(expectedItems));
        }

        [Test]
        public void InequalItemsSingleMeasure()
        {
            // all items will only have one measurement but the levels will all be different
            // by an even spread based on how many items there are
            var measurementIncrement = 1.0 / this.items.Count;
            for (var i = 0; i < this.items.Count; i++)
            {
                var measurementLevel = (i + 1) * measurementIncrement;
                this.items[i].SetXLevel(measurementLevel);
            }

            // set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            DecisionLogic.SetBaseWeighting(0.0);

            var biasedItem = new TestBiasedItem(1.0, 0.0);
            var chosenItems = new List<TestMeasurableItem>();

            // the numbers generated need to reflect the range of measurement levels in the items
            // if there are 8 items...
            // - 8/8 pheromone -> 8 results
            // - 7/8 pheromone -> 7 results
            // - 6/8 pheromone -> 6 results
            // - ...
            // - 1/8 pheromone -> 1 result
            // total number of results: 1 + 2 + 3 + 4 + ... = n(n + 1)/2 [sum of integers from 1 - n]
            var numberOfResults = this.items.Count * (this.items.Count + 1) / 2.0;
            var nextDouble = 0.0;
            for (var i = 0; i < numberOfResults; i++)
            {
                RandomNumberGenerator.OverrideNextDouble = nextDouble;
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, biasedItem));
                nextDouble += 1.0 / numberOfResults;
            }

            // expecting each organism to be chosen a number of times proportional to the single measurement level used
            var expectedItemCounts = new Dictionary<TestMeasurableItem, int>();
            for (var i = 0; i < this.items.Count; i++)
            {
                expectedItemCounts.Add(this.items.ElementAt(i), i + 1);
            }

            var actualItemCounts = new Dictionary<TestMeasurableItem, int>();
            foreach (var item in this.items)
            {
                var numberOfTimesChosen = chosenItems.Count(chosenItem => chosenItem.Equals(item));
                actualItemCounts.Add(item, numberOfTimesChosen);
            }

            Assert.That(actualItemCounts, Is.EqualTo(expectedItemCounts));
        }

        [Test]
        public void InequalItemsMultipleBalancingMeasures()
        {
            // all items will have two measures, the second being the inverse of the first
            // this will balance the overall weighting and give all items a perfectly even chance
            var measurementIncrement = 1.0 / this.items.Count;
            for (var i = 0; i < this.items.Count; i++)
            {
                var measurementLevel = (i + 1) * measurementIncrement;
                this.items[i].SetXLevel(measurementLevel);
                this.items[i].SetYLevel(1.0 - measurementLevel);
            }

            // set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            DecisionLogic.SetBaseWeighting(0.0);

            // bias of both measurements are the same in order for them to balance
            var biasedItem = new TestBiasedItem(1.0, 1.0);
            var chosenItems = new List<TestMeasurableItem>();

            var nextDouble = 0.0;
            for (var i = 0; i < this.items.Count; i++)
            {
                RandomNumberGenerator.OverrideNextDouble = nextDouble;
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, biasedItem));
                nextDouble += 1.0 / this.items.Count;
            }

            // items are not equal, but the bias should evenly distribute the selection of items
            var expectedItems = this.items;
            var actualItems = chosenItems;
            Assert.That(actualItems, Is.EqualTo(expectedItems));
        }

        [Test]
        public void InequalItemsMultipleUnbalancingMeasures()
        {
            // all items will have two measures, the second being double the inverse of the first
            // this imbalance will be corrected by the bias, showing that the bias has the desired effect
            var measurementIncrement = 1.0 / this.items.Count;
            for (var i = 0; i < this.items.Count; i++)
            {
                var measurementLevel = (i + 1) * measurementIncrement;
                this.items[i].SetXLevel(measurementLevel / 2.0);
                this.items[i].SetYLevel(1 - measurementLevel);
            }

            // set the base weighting to 0 (so that chance of being chosen is based directly on measurment level * bias)
            DecisionLogic.SetBaseWeighting(0.0);

            // pheromone bias is double that of health bias to compensate for the halving of the measurement level
            var biasedItem = new TestBiasedItem(2.0, 1.0);
            var chosenItems = new List<TestMeasurableItem>();

            var nextDouble = 0.0;
            for (var i = 0; i < this.items.Count; i++)
            {
                RandomNumberGenerator.OverrideNextDouble = nextDouble;
                chosenItems.Add(DecisionLogic.MakeDecision(this.items, biasedItem));
                nextDouble += 1.0 / this.items.Count;
            }

            // items are not equal, but the bias should evenly compensate for the disproportionate measurements
            // and evenly distribute the selection of items
            var expectedItems = this.items;
            var actualItems = chosenItems;
            Assert.That(actualItems, Is.EqualTo(expectedItems));
        }                                                       

        private class TestMeasurableItem : IMeasurable<TestMeasure>
        {
            private readonly string identifier;
            private readonly Measurement xMeasurement;
            private readonly Measurement yMeasurement;

            public IMeasurementData MeasurementData
            {
                get
                {
                    return new MeasurementData(new List<Measurement> { this.xMeasurement, this.yMeasurement });
                }
            }

            public double GetLevel(TestMeasure testMeasure)
            {
                return this.MeasurementData.GetLevel(testMeasure);
            }

            public TestMeasurableItem(string identifier)
            {
                this.identifier = identifier;
                this.xMeasurement = new Measurement(TestMeasure.X, 1.0);
                this.yMeasurement = new Measurement(TestMeasure.Y, 1.0);
            }

            public void SetXLevel(double pheromoneLevel)
            {
                this.xMeasurement.SetLevel(pheromoneLevel);
            }

            public void SetYLevel(double healthLevel)
            {
                this.yMeasurement.SetLevel(healthLevel);
            }

            public override string ToString()
            {
                return this.identifier;
            }
        }

        private class TestBiasedItem : IBiased<TestMeasure>
        {
            private readonly double xBias;
            private readonly double yBias;

            public Dictionary<TestMeasure, double> MeasureBiases
            {
                get
                {
                    // there needs to be a bias for each measure (will be applied to all items)
                    // but will have no effect when there is only one measure
                    return new Dictionary<TestMeasure, double> { { TestMeasure.X, this.xBias }, { TestMeasure.Y, this.yBias } };
                }
            }

            public TestBiasedItem(double xBias, double yBias)
            {
                this.xBias = xBias;
                this.yBias = yBias;
            }

            public void SetMeasureBias(TestMeasure testMeasure, double bias)
            {
                this.MeasureBiases[testMeasure] = bias;
            }
        }

        private class TestMeasure : Enumeration, IMeasure
        {
            public static readonly TestMeasure X = new TestMeasure(0, "x");
            public static readonly TestMeasure Y = new TestMeasure(1, "y");

            private TestMeasure(int value, string friendlyString)
                : base(value, friendlyString)
            {
            }
        }
    }
}


