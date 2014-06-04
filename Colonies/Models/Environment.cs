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

        public double GetLevel(EnvironmentMeasure measure)
        {
            return this.measurement.GetLevel(measure);
        }

        public void SetLevel(EnvironmentMeasure measure, double level)
        {
            this.measurement.SetLevel(measure, level);
        }

        public bool IncreaseLevel(EnvironmentMeasure measure, double increment)
        {
            return this.measurement.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(EnvironmentMeasure measure, double decrement)
        {
            return this.measurement.DecreaseLevel(measure, decrement);
        }

        public static bool IsPotentialHazard(EnvironmentMeasure measure)
        {
            return HazardMeasures().Any(hazardMeasure => hazardMeasure.Equals(measure));
        }

        public static IEnumerable<EnvironmentMeasure> Measures()
        {
            return new List<EnvironmentMeasure> { EnvironmentMeasure.Pheromone, EnvironmentMeasure.Nutrient, EnvironmentMeasure.Mineral, EnvironmentMeasure.Obstruction, EnvironmentMeasure.Damp, EnvironmentMeasure.Heat, EnvironmentMeasure.Poison };
        }

        public static IEnumerable<EnvironmentMeasure> HazardMeasures()
        {
            return new List<EnvironmentMeasure> { EnvironmentMeasure.Damp, EnvironmentMeasure.Heat, EnvironmentMeasure.Poison };
        }
        
        public override string ToString()
        {
            return this.measurement.ToString();
        }
    }
}
