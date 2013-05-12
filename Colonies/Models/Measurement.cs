namespace Colonies.Models
{
    using System.Collections.Generic;

    public class Measurement
    {
        public Measure Measure { get; private set; }
        public double Level { get; private set; }
        public double Bias { get; private set; }

        public Measurement(Measure measure, double level)
        {
            this.Measure = measure;
            this.Level = level;
        }

        public void IncreaseLevel(double value)
        {
            this.Level += value;
        }

        public void DecreaseLevel(double value)
        {
            this.Level -= value;
        }

        public void SetBias(double bias)
        {
            this.Bias = bias;
        }
    }
}