namespace Colonies.Models
{
    public class Stimulus
    {
        public readonly double PheromoneLevel;

        public Stimulus(double pheromoneLevel)
        {
            this.PheromoneLevel = pheromoneLevel;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.PheromoneLevel);
        }
    }
}
