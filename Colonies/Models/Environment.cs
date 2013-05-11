﻿namespace Colonies.Models
{
    using System.Collections.Generic;

    public sealed class Environment
    {
        public Terrain Terrain { get; private set; }
        public double PheromoneLevel { get; private set; }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void IncreasePheromoneLevel(double levelIncrease)
        {
            this.PheromoneLevel += levelIncrease;
        }

        public void DecreasePheromoneLevel(double levelDecrease)
        {
            // TODO: a similar thing with level increase > 1?
            this.PheromoneLevel -= levelDecrease;
            if (this.PheromoneLevel < 0)
            {
                this.PheromoneLevel = 0;
            }
        }

        public List<Stimulus> GetStimulus()
        {
            return new List<Stimulus> { new Stimulus(Factor.Pheromone, this.PheromoneLevel) };
        }
        
        public override string ToString()
        {
            return string.Format("{0} <Ph:{1}>", this.Terrain.ToString(), this.PheromoneLevel);
        }
    }
}
