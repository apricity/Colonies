namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public sealed class Organism : IOrganism
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }

        private readonly MeasurementData measurementData;
        public IMeasurementData MeasurementData
        {
            get
            {
                return this.measurementData;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.measurementData.GetLevel(OrganismMeasure.Health) > 0.0;
            }
        }

        private bool pheromoneEnabled;
        public bool IsDepositingPheromone
        {
            get
            {
                return this.pheromoneEnabled && this.IsAlive;
            }
        }

        private bool soundEnabled;
        public bool IsEmittingSound
        {
            get
            {
                return this.soundEnabled && this.IsAlive;
            }
        }

        public Intention Intention { get; private set; }
        public Dictionary<EnvironmentMeasure, double> MeasureBiases
        {
            get
            {
                return this.Intention.EnvironmentBiases;
            }
        }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Measurement(OrganismMeasure.Health, 1.0);
            this.measurementData = new MeasurementData(new List<Measurement> { health });
            this.Intention = Intention.Eat;
        }

        public double GetLevel(OrganismMeasure testMeasure)
        {
            return this.measurementData.GetLevel(testMeasure);
        }

        public void SetLevel(OrganismMeasure measure, double level)
        {
            this.measurementData.SetLevel(measure, level);
        }

        public bool IncreaseLevel(OrganismMeasure measure, double increment)
        {
            return this.measurementData.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(OrganismMeasure measure, double decrement)
        {
            return this.measurementData.DecreaseLevel(measure, decrement);
        }

        public void EnableSound()
        {
            this.soundEnabled = true;
        }

        public void DisableSound()
        {
            this.soundEnabled = false;
        }

        public void EnablePheromone()
        {
            this.pheromoneEnabled = true;
        }

        public void DisablePheromone()
        {
            this.pheromoneEnabled = false;
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