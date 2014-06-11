namespace Wacton.Colonies.Utilities
{
    using System;

    public static class RandomNumberGenerator
    {
        private static Random random = new Random();

        public static double? OverrideNextDouble { get; set; }

        public static double RandomDouble(double range)
        {
            if (OverrideNextDouble.HasValue)
            {
                return (double)OverrideNextDouble * range;
            }

            return random.NextDouble() * range;
        }
    }
}
