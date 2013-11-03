namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Linq;

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
                return Hazards().Any(hazard => this.Measurement.HasCondition(hazard));
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

        public static bool IsPotentialHazard(Measure measure)
        {
            return Hazards().Any(hazard => hazard.Measure.Equals(measure));
        }

        private static IEnumerable<Condition> Hazards()
        {
            var dampHazard = new Condition(Measure.Damp, 1.0);
            var heatHazard = new Condition(Measure.Heat, 1.0);
            var poisonHazard = new Condition(Measure.Poison, 1.0);
            return new List<Condition> { dampHazard, heatHazard, poisonHazard };
        }
        
        public override string ToString()
        {
            return this.Measurement.ToString();
        }
    }
}
