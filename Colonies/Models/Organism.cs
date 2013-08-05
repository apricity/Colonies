namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Interfaces;

    public sealed class Organism : IMeasurable, IBiased
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Condition Health { get; private set; }
        public bool IsDepositingPheromones { get; private set; }
        public double PheromoneBias { get; private set; }

        public bool IsAlive
        {
            get
            {
                return !(this.Health.Level <= 0.0);
            }
        }

        public Organism(string name, Color color, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;

            this.Health = new Condition(Measure.Health, 1.0);

            this.PheromoneBias = 10;

            // TODO: depositing pheromones should probably not be something that is handled through construction (it will probably be very dynamic)
            this.IsDepositingPheromones = isDepostingPheromones;
        }

        public void IncreaseHealth(double increaseLevel)
        {
            this.Health.IncreaseLevel(increaseLevel);
            if (this.Health.Level > 1)
            {
                this.Health.SetLevel(1.0);
            }
        }

        public void DecreaseHealth(double decreaseLevel)
        {
            this.Health.DecreaseLevel(decreaseLevel);
            if (this.Health.Level < 0)
            {
                this.Health.SetLevel(0.0);
            }

            if (!this.IsAlive)
            {
                this.IsDepositingPheromones = false;
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
            return string.Format("{0}: {1} {2}", this.Name, this.Health, this.Color);
        }
    }
}