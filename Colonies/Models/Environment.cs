namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;

    using Wacton.Colonies.Interfaces;

    public sealed class Environment : IMeasurable
    {
        public Terrain Terrain { get; private set; }
        public bool IsObstructed { get; private set; }
        public Condition Pheromone { get; private set; }

        public Environment(Terrain terrain, bool isObstructed)
        {
            this.Terrain = terrain;
            this.IsObstructed = isObstructed;
            this.Pheromone = new Condition(Measure.Pheromone, 0);
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void SetObstructed(bool isObstructed)
        {
            this.IsObstructed = isObstructed;
        }

        public void IncreasePheromoneLevel(double levelIncrease)
        {
            this.Pheromone.IncreaseLevel(levelIncrease);
        }

        public bool DecreasePheromoneLevel(double levelDecrease)
        {
            this.Pheromone.DecreaseLevel(levelDecrease);
            if (this.Pheromone.Level < 0)
            {
                this.Pheromone.SetLevel(0.0);
                return false;
            }

            return true;
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
