namespace Colonies.Models
{
    public class Stimulus
    {
        public readonly Factor Factor;
        public readonly double Level;
        public double Bias;

        public Stimulus(Factor factor, double level)
        {
            this.Factor = factor;
            this.Level = level;
            this.Bias = 0;
        }

        public void SetBias(double bias)
        {
            this.Bias = bias;
        }
    }
}