namespace Colonies.Models
{
    public sealed class Environment
    {
        public Terrain Terrain { get; set; }
        public double PheromoneLevel { get; private set; }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void IncreasePheromoneLevel(double levelIncrease)
        {
            this.PheromoneLevel += levelIncrease;
        }

        public void DecreasePheromoneLevel(double levelDecrease)
        {
            this.PheromoneLevel -= levelDecrease;
        }
        
        public override string ToString()
        {
            return string.Format("{0} <Ph:{1}>", this.Terrain.ToString(), this.PheromoneLevel);
        }
    }
}
