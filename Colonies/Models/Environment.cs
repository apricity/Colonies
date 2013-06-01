namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;

    using Wacton.Colonies.Interfaces;

    public sealed class Environment : IMeasurable
    {
        public Terrain Terrain { get; private set; }
        public Condition Pheromone { get; private set; }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;
            this.Pheromone = new Condition(Measure.Pheromone, 0);
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
            if (this.Pheromone.Level < 0)
            {
                this.Pheromone.SetLevel(0.0);
            }
        }

        public Measurement GetMeasurement()
        {
            return new Measurement( new List<Condition> { this.Pheromone });
        }
        
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Terrain, this.Pheromone);
        }
    }
}
