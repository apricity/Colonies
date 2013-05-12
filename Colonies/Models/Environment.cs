namespace Colonies.Models
{
    using System.Collections.Generic;

    public sealed class Environment : IMeasurable
    {
        public Terrain Terrain { get; private set; }
        public Measurement Pheromone { get; private set; }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;
            this.Pheromone = new Measurement(Measure.Pheromone, 0);
        }

        public List<Measurement> GetMeasurements()
        {
            return new List<Measurement> { this.Pheromone };
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void IncreasePheromoneLevel(double levelIncrease)
        {
            this.Pheromone.IncreaseLevel(levelIncrease);
        }

        public void DecreasePheromoneLevel(double levelDecrease)
        {
            this.Pheromone.DecreaseLevel(levelDecrease);
        }
        
        public override string ToString()
        {
            return string.Format("{0} <Ph:{1}>", this.Terrain.ToString(), this.Pheromone.Level);
        }
    }
}
