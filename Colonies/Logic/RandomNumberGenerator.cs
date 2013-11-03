namespace Wacton.Colonies.Logic
{
    using System;

    public static class RandomNumberGenerator
    {
        private static Random random = new Random();

        public static double RandomDouble(double multiplier)
        {
            return random.NextDouble() * multiplier;
        }

        public static void SetRandom(Random newRandom)
        {
            random = newRandom;
        }
    }
}
