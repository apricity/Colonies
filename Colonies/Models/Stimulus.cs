namespace Colonies.Models
{
    public class Stimulus
    {
        public readonly Measure Measure;
        public readonly double Level;
        public double Bias;

        public Stimulus(Measure measure, double level)
        {
            this.Measure = measure;
            this.Level = level;
            this.Bias = 0;
        }

        public void SetBias(double bias)
        {
            this.Bias = bias;
        }
    }
}