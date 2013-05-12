namespace Colonies.Models
{
    public class Condition
    {
        public Measure Measure { get; private set; }
        public double Level { get; private set; }
        public double Bias { get; private set; }

        public Condition(Measure measure, double level)
        {
            this.Measure = measure;
            this.Level = level;
        }

        public void SetLevel(double level)
        {
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