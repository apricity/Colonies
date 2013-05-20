namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies.Logic;

    public sealed class Organism : IMeasurable, IBiased
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Condition Health { get; private set; }
        public bool IsDepositingPheromones { get; private set; }
        public double PheromoneBias { get; private set; }

        public Organism(string name, Color color, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;

            this.Health = new Condition(Measure.Health, 1.0);

            this.PheromoneBias = 10;

            // TODO: depositing pheromones should probably not be something that is handled through construction (it will probably be very dynamic)
            this.IsDepositingPheromones = isDepostingPheromones;
        }

        public void DecreaseHealth(double decreaseLevel)
        {
            this.Health.DecreaseLevel(decreaseLevel);
            if (this.Health.Level < 0)
            {
                this.Health.SetLevel(0.0);
            }
        }

        public Measurement GetMeasurement()
        {
            return new Measurement(new List<Condition> { this.Health });
        }

        public Dictionary<Measure, double> GetMeasureBiases()
        {
            return new Dictionary<Measure, double> { { Measure.Pheromone, this.PheromoneBias } };
        }

        // TODO: take Measure as another parameter, and update a dictionary of Measure -> bias accordingly
        public void SetPheromoneBias(double bias)
        {
            this.PheromoneBias = bias;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", this.Name, this.Health.Level * 100, this.Color);
        }
    }
}