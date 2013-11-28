namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public sealed class Environment : IEnvironment
    {
        private readonly Measurement measurement;
        public IMeasurement Measurement
        {
            get
            {
                return this.measurement;
            }
        }

        public bool IsHazardous
        {
            get
            {
                return HazardMeasures().Any(hazardMeasure => this.measurement.GetLevel(hazardMeasure).Equals(1.0));
            }
        }

        public Environment()
        {
            var conditions = new List<Condition>();
            foreach (var measure in Measures())
            {
                conditions.Add(new Condition(measure, 0));
            }

            this.measurement = new Measurement(conditions);
        }

        public double GetLevel(Measure measure)
        {
            return this.measurement.GetLevel(measure);
        }

        public void SetLevel(Measure measure, double level)
        {
            this.measurement.SetLevel(measure, level);
        }

        public bool IncreaseLevel(Measure measure, double increment)
        {
            return this.measurement.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(Measure measure, double decrement)
        {
            return this.measurement.DecreaseLevel(measure, decrement);
        }

        public static bool IsPotentialHazard(Measure measure)
        {
            return HazardMeasures().Any(hazardMeasure => hazardMeasure.Equals(measure));
        }

        public static IEnumerable<Measure> Measures()
        {
            return new List<Measure> { Measure.Pheromone, Measure.Nutrient, Measure.Mineral, Measure.Obstruction, Measure.Damp, Measure.Heat, Measure.Poison };
        }

        public static IEnumerable<Measure> HazardMeasures()
        {
            return new List<Measure> { Measure.Damp, Measure.Heat, Measure.Poison };
        }
        
        public override string ToString()
        {
            return this.measurement.ToString();
        }
    }
}
