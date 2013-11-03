namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public sealed class Environment : IMeasurable
    {
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

        public bool IsHazardous
        {
            get
            {
                return !(this.GetLevel(Measure.Damp) < 1.0) 
                       || !(this.GetLevel(Measure.Heat) < 1.0)
                       || !(this.GetLevel(Measure.Poison) < 1.0);
            }
        }

        public Environment()
        {
            var pheromone = new Condition(Measure.Pheromone, 0);
            var nutrient = new Condition(Measure.Nutrient, 0);
            var mineral = new Condition(Measure.Mineral, 0);
            var obstruction = new Condition(Measure.Obstruction, 0);
            var damp = new Condition(Measure.Damp, 0);
            var heat = new Condition(Measure.Heat, 0);
            var poison = new Condition(Measure.Poison, 0);

            this.Measurement = new Measurement(new List<Condition> { pheromone, nutrient, mineral, obstruction, damp, heat, poison });
        }

        public double GetLevel(Measure measure)
        {
            return this.Measurement.GetLevel(measure);
        }

        public void SetLevel(Measure measure, double level)
        {
            this.Measurement.SetLevel(measure, level);
        }

        public bool IncreaseLevel(Measure measure, double increment)
        {
            return this.Measurement.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(Measure measure, double decrement)
        {
            return this.Measurement.DecreaseLevel(measure, decrement);
        }
        
        public override string ToString()
        {
            return this.Measurement.ToString();
        }
    }
}
