namespace Wacton.Colonies.Utilities
{
    using System;

    public static class RandomNumberGenerator
    {
        private static Random random = new Random();

        public static double? OverrideNextDouble { get; set; }

        public static double RandomDouble(double multiplier)
        {
            if (OverrideNextDouble.HasValue)
            {
                return (double)OverrideNextDouble * multiplier;
            }

            return random.NextDouble() * multiplier;
        }
    }
}
