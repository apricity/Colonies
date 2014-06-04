namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public sealed class Organism : IOrganism
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public bool IsDepositingPheromones { get; private set; }

        private readonly Measurement measurement;
        public IMeasurement Measurement
        {
            get
            {
                return this.measurement;
            }
        }

        public Dictionary<EnvironmentMeasure, double> MeasureBiases { get; private set; } 

        public bool IsAlive
        {
            get
            {
                return this.measurement.GetLevel(OrganismMeasure.Health) > 0.0;
            }
        }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Condition(OrganismMeasure.Health, 1.0);
            this.measurement = new Measurement(new List<Condition> { health });
            this.MeasureBiases = new Dictionary<EnvironmentMeasure, double>
                                     {
                                         { EnvironmentMeasure.Pheromone, 10 },
                                         { EnvironmentMeasure.Nutrient, 0 },
                                         { EnvironmentMeasure.Mineral, 0 },
                                         { EnvironmentMeasure.Damp, 0 },
                                         { EnvironmentMeasure.Heat, 0 },
                                         { EnvironmentMeasure.Poison, 0 },
                                         { EnvironmentMeasure.Obstruction, 0}
                                     };
        }

        public double GetLevel(OrganismMeasure measure)
        {
            return this.measurement.GetLevel(measure);
        }

        public void SetLevel(OrganismMeasure measure, double level)
        {
            this.measurement.SetLevel(measure, level);
        }

        public bool IncreaseLevel(OrganismMeasure measure, double increment)
        {
            return this.measurement.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(OrganismMeasure measure, double decrement)
        {
            return this.measurement.DecreaseLevel(measure, decrement);
        }

        public static IEnumerable<OrganismMeasure> Measures()
        {
            return new List<OrganismMeasure> { OrganismMeasure.Health };
        }

        public void SetMeasureBias(EnvironmentMeasure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", this.Name, this.GetLevel(OrganismMeasure.Health), this.Color);
        }
    }
}