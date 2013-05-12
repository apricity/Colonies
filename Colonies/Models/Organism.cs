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
        private readonly IDecisionLogic decisionLogic;

        public Dictionary<Measure, double> MeasureBiases { get; private set; }

        public Organism(string name, Color color, IDecisionLogic decisionLogic, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;
            this.decisionLogic = decisionLogic;

            this.Health = new Condition(Measure.Health, 1.0);
            this.MeasureBiases = new Dictionary<Measure, double> { { Measure.Pheromone, 10 } };

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

        public Measurement ProcessEnvironmentMeasurements(List<Measurement> environmentMeasurements, Random random)
        {
            // add bias to the measurements, according to how the organism weights each measure
            foreach (var environmentMeasurement in environmentMeasurements)
            {
                foreach (var condition in environmentMeasurement.Conditions)
                {
                    condition.SetBias(this.MeasureBiases[condition.Measure]);
                }
            }

            var chosenMeasurement = this.decisionLogic.MakeDecision(environmentMeasurements, random);
            return chosenMeasurement;
        }

        public Measurement GetMeasurement()
        {
            return new Measurement(new List<Condition> { this.Health });
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", this.Name, this.Health.Level * 100, this.Color);
        }
    }
}