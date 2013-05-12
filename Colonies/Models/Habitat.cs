﻿namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class Habitat
    {
        public Environment Environment { get; private set; }
        public Organism Organism { get; private set; }

        public Habitat(Environment environment, Organism organism)
        {
            this.Environment = environment;
            this.Organism = organism;
        }

        public void AddOrganism(Organism organism)
        {
            this.Organism = organism;
        }

        public void RemoveOrganism()
        {
            this.Organism = null;
        }

        public bool ContainsOrganism()
        {
            return this.Organism != null;
        }

        public bool ContainsImpassable()
        {
            return this.Environment.Terrain == Terrain.Impassable;
        }

        // TODO: is this necessary?
        public Measurement GetEnvironmentMeasurement()
        {
            // TODO: take into account organisms before return the measure?
            return this.Environment.GetMeasurement();
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Environment, this.Organism);
        }
    }
}
