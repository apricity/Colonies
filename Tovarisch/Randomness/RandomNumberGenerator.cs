namespace Wacton.Tovarisch.Randomness
{
    using System;

    public static class RandomNumberGenerator
    {
        private static readonly Random Random = new Random();

        public static double? OverrideNextDouble { private get; set; }

        public static double DoubleBetween(double range)
        {
            if (OverrideNextDouble.HasValue)
            {
                return (double)OverrideNextDouble * range;
            }

            return Random.NextDouble() * range;
        }
    }
}
