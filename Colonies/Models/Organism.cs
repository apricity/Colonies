namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public abstract class Organism : IOrganism
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Intention Intention { get; protected set; }

        public string IntentionString
        {
            get
            {
                return this.Intention.ToString();
            }
        }

        private readonly MeasurementData<OrganismMeasure> measurementData;
        public IMeasurementData<OrganismMeasure> MeasurementData
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

        public bool IsReproducing
        {
            get
            {
                return this.IsAlive && this.Intention.Equals(Intention.Reproduce);
            }
        }

        public abstract bool NeedsAssistance { get; }

        public bool IsDepositingPheromone
        {
            get
            {
                return this.IsAlive && this.Intention.Equals(Intention.Nourish);
            }
        }

        public Dictionary<EnvironmentMeasure, double> MeasureBiases
        {
            get
            {
                return this.Intention.EnvironmentBiases;
            }
        }

        public Measurement<EnvironmentMeasure> Inventory { get; protected set; }

        protected Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Measurement<OrganismMeasure>(OrganismMeasure.Health, 1.0);
            this.measurementData = new MeasurementData<OrganismMeasure>(new List<Measurement<OrganismMeasure>> { health });
        }

        public Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismLogic.ProcessMeasurableEnvironment(this, measurableEnvironment);
        }

        public abstract void RefreshIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        public double GetLevel(OrganismMeasure measure)
        {
            return this.measurementData.GetLevel(measure);
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

        public override string ToString()
        {
            return string.Format("{0}: {1} {2} {3}", this.Name, this.GetLevel(OrganismMeasure.Health), this.Intention, this.Color);
        }
    }
}