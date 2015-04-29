namespace Wacton.Colonies.Tests
{
    using System;
    using System.Windows.Media;

    using NUnit.Framework;

    using Wacton.Colonies.UI.Converters;

    [TestFixture]
    public class ColorTests
    {
        private readonly DoubleToHealthLevelBrushConverter doubleToHealthLevelBrushConverter = new DoubleToHealthLevelBrushConverter();

        [Test]
        public void RedToYellowHealthColors()
        {
            var healthLevel = 0.0;
            var previousBrush = this.GetBrush(healthLevel);
            Assert.That(previousBrush.Color, Is.EqualTo(Color.FromRgb(255, 0, 0)));

            for (healthLevel = 0.0001; healthLevel <= 0.5; healthLevel = Math.Round(healthLevel + 0.0001, 4))
            {
                var currentBrush = this.GetBrush(healthLevel);

                // check that the green channel has, at most, incremented by one
                Assert.That(currentBrush.Color.R, Is.EqualTo(previousBrush.Color.R));
                Assert.That(currentBrush.Color.G, Is.InRange(previousBrush.Color.G, previousBrush.Color.G + 1));
                Assert.That(currentBrush.Color.B, Is.EqualTo(previousBrush.Color.B));

                previousBrush = currentBrush;
            }

            // ensure that the colors has progressed through the entire range of channel values (not just one)
            Assert.That(previousBrush.Color, Is.EqualTo(Color.FromRgb(255, 255, 0)));
        }

        [Test]
        public void YellowToGreenHealthColors()
        {
            var healthLevel = 0.5;
            var previousBrush = this.GetBrush(healthLevel);
            Assert.That(previousBrush.Color, Is.EqualTo(Color.FromRgb(255, 255, 0)));

            for (healthLevel = 0.5001; healthLevel <= 1.0; healthLevel = Math.Round(healthLevel + 0.0001, 4))
            {
                var currentBrush = this.GetBrush(healthLevel);

                // check that the color has, at most, incremented by one channel value
                Assert.That(currentBrush.Color.R, Is.InRange(previousBrush.Color.R - 1, previousBrush.Color.R));
                Assert.That(currentBrush.Color.G, Is.EqualTo(previousBrush.Color.G));
                Assert.That(currentBrush.Color.B, Is.EqualTo(previousBrush.Color.B));

                previousBrush = currentBrush;
            }

            // ensure that the colors has progressed through the entire range of channel values (not just one)
            Assert.That(previousBrush.Color, Is.EqualTo(Color.FromRgb(0, 255, 0)));
        }

        private SolidColorBrush GetBrush(double healthLevel)
        {
            return (SolidColorBrush)this.doubleToHealthLevelBrushConverter.Convert(healthLevel, null, null, null);
        }
    }
}
