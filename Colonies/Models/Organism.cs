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

        public Dictionary<Measure, double> MeasureBiases { get; private set; } 

        public bool IsAlive
        {
            get
            {
                return this.measurement.GetLevel(Measure.Health) > 0.0;
            }
        }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Condition(Measure.Health, 1.0);
            this.measurement = new Measurement(new List<Condition> { health });
            this.MeasureBiases = new Dictionary<Measure, double>
                                     {
                                         { Measure.Pheromone, 10 },
                                         { Measure.Nutrient, 0 },
                                         { Measure.Mineral, 0 },
                                         { Measure.Damp, 0 },
                                         { Measure.Heat, 0 },
                                         { Measure.Poison, 0 },
                                         { Measure.Obstruction, 0}
                                     };
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

        public static IEnumerable<Measure> Measures()
        {
            return new List<Measure> { Measure.Health };
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", this.Name, this.GetLevel(Measure.Health), this.Color);
        }
    }
}