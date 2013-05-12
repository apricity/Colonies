namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies.Logic;

    public sealed class Organism : IMeasurable
    {
        // TODO: this should not be hard-coded, perhaps
        private const int PheromoneWeighting = 10;

        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Measurement Health { get; private set; }
        public bool IsDepositingPheromones { get; private set; }
        private readonly IDecisionLogic decisionLogic;

        public Organism(string name, Color color, IDecisionLogic decisionLogic, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;
            this.decisionLogic = decisionLogic;

            this.Health = new Measurement(Measure.Health, 1.0);

            // TODO: depositing pheromones should probably not be something that is handled through construction (it will probably be very dynamic)
            this.IsDepositingPheromones = isDepostingPheromones;
        }

        public List<Measurement> GetMeasurements()
        {
            return new List<Measurement> { this.Health };
        }

        public void DecreaseHealth(double decreaseLevel)
        {
            this.Health.DecreaseLevel(decreaseLevel);
        }

        // TODO: encapsulate "List<Measurement>" in a "Measurements" class?
        public List<Measurement> ProcessEnvironmentMeasurements(List<List<Measurement>> environmentMeasurements, Random random)
        {
            // add bias to the measurements, according to how the organism weights each measure
            foreach (var environmentMeasurement in environmentMeasurements.SelectMany(measurements => measurements))
            {
                environmentMeasurement.SetBias(PheromoneWeighting);
            }

            var chosenMeasurements = this.decisionLogic.MakeDecision(environmentMeasurements, random);
            return chosenMeasurements;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", this.Name, this.Health.Level * 100, this.Color);
        }
    }
}