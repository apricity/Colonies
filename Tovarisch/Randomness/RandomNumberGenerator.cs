namespace Wacton.Tovarisch.Randomness
{
    using System;

    public static class RandomNumberGenerator
    {
        private static readonly Random Random = new Random();

        public static int? OverrideNextInteger { private get; set; }
        public static double? OverrideNextDouble { private get; set; }

        public static int IntegerBetween(int minimum, int maximum)
        {
            if (OverrideNextInteger.HasValue)
            {
                if (OverrideNextInteger.Value < minimum || OverrideNextInteger.Value >= maximum)
                {
                    throw new InvalidOperationException("Overridden next random integer is out of range");
                }

                return OverrideNextInteger.Value;
            }

            return Random.Next(minimum, maximum);
        }

        public static double DoubleBetween(double minimum, double maximum)
        {
            var range = maximum - minimum;

            if (OverrideNextDouble.HasValue)
            {
                return (OverrideNextDouble.Value * range) + minimum;
            }

            return (Random.NextDouble() * range) + minimum;
        }
    }
}
