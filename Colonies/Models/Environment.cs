namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public sealed class Environment : IMeasurable
    {
        public Terrain Terrain { get; private set; }
        public Measurement Measurement { get; private set; }

        public bool IsObstructed
        {
            get
            {
                return this.GetLevel(Measure.Obstruction) > 0;
            }
        }

        public bool HasNutrient
        {
            get
            {
                return this.GetLevel(Measure.Nutrient) > 0;
            }
        }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;

            var pheromone = new Condition(Measure.Pheromone, 0);
            var nutrient = new Condition(Measure.Nutrient, 0);
            var mineral = new Condition(Measure.Mineral, 0);
            var obstruction = new Condition(Measure.Obstruction, 0);

            var damp = this.Terrain.Equals(Terrain.Water)
                            ? new Condition(Measure.Damp, 1)
                            : new Condition(Measure.Damp, 0);

            var heat = this.Terrain.Equals(Terrain.Fire)
                            ? new Condition(Measure.Heat, 1)
                            : new Condition(Measure.Heat, 0);

            this.Measurement = new Measurement(new List<Condition> { pheromone, nutrient, mineral, obstruction, damp, heat });
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public double GetLevel(Measure measure)
        {
            return this.Measurement.GetLevel(measure);
        }

        public void SetLevel(Measure measure, double level)
        {
            this.EnsureModifiable(measure);
            this.Measurement.SetLevel(measure, level);
        }

        public bool IncreaseLevel(Measure measure, double increment)
        {
            this.EnsureModifiable(measure);
            return this.Measurement.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(Measure measure, double decrement)
        {
            this.EnsureModifiable(measure);
            return this.Measurement.DecreaseLevel(measure, decrement);
        }

        // TODO: use a dictionary to keep track of restrictions if this grows later on
        private void EnsureModifiable(Measure measure)
        {
            if (this.Terrain.Equals(Terrain.Water) && measure.Equals(Measure.Damp)
                || this.Terrain.Equals(Terrain.Fire) && measure.Equals(Measure.Heat))
            {
                //throw new InvalidOperationException(string.Format("Modification of {0} - {1} is disallowed", this.Terrain, measure));
            }
        }
        
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Terrain, this.Measurement);
        }
    }
}
