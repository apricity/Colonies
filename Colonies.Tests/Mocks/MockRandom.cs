namespace Wacton.Colonies.Tests.Mocks
{
    using System;

    public class MockRandom : Random
    {
        private double nextDouble;

        public void SetNextDouble(double value)
        {
            this.nextDouble = value;
        }

        public override double NextDouble()
        {
            return this.nextDouble;
        }
    }
}
