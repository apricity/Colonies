using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoniesTests.Mocks
{
    public class MockRandom : Random
    {
        private double nextDouble;

        public void SetNextDouble(double value)
        {
            this.nextDouble = value;
        }

        public override double NextDouble()
        {
            return nextDouble;
        }
    }
}
