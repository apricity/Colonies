namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public abstract class Organism : IOrganism
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Inventory Inventory { get; private set; }
        public double Age { get; private set; }

        private Intention intention;
        public Intention Intention
        {
            get
            {
                return this.IsAlive ? this.intention : Intention.Dead;
            }
            private set
            {
                this.intention = value;
            }
        }

        private double IntentionStartAge { get; set; }
        private double IntentionDuration
        {
            get
            {
                return Math.Round(this.Age - this.IntentionStartAge, 4);
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

        public abstract bool IsCalling { get; }

        public bool IsDepositingPheromone
        {
            get
            {
                // TODO: intention duration limit should probably be relative to the size of the ecosystem
                return this.Intention.Equals(Intention.Nourish) && this.IntentionDuration <= 50;
            }
        }

        public Dictionary<EnvironmentMeasure, double> MeasureBiases
        {
            get
            {
                return this.Intention.EnvironmentBiases;
            }
        }

        protected Organism(string name, Color color, Inventory inventoryType, Intention initialIntention)
        {
            this.Name = name;
            this.Color = color;
            this.Inventory = inventoryType;

            var health = new Measurement<OrganismMeasure>(OrganismMeasure.Health, 1.0);
            var inventory = new Measurement<OrganismMeasure>(OrganismMeasure.Inventory, 0.0);
            this.measurementData = new MeasurementData<OrganismMeasure>(new List<Measurement<OrganismMeasure>> { health, inventory });

            this.Intention = Intention.None;
            this.UpdateIntention(initialIntention);
        }

        public void IncrementAge(double increment)
        {
            this.Age += increment;
        }

        public abstract Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        public void UpdateIntention(Intention newIntention)
        {
            if (!newIntention.IsCompatibleWith(this.Inventory))
            {
                throw new InvalidOperationException(
                    string.Format("Intention {0} is incompatible with inventory {1} (requires inventory {2})", 
                                  newIntention, this.Inventory, newIntention.RequiredInventory));
            }

            if (this.Intention.Equals(newIntention))
            {
                return;
            }

            this.Intention = newIntention;
            this.IntentionStartAge = this.Age;
        }

        public double GetLevel(OrganismMeasure measure)
        {
            return this.measurementData.GetLevel(measure);
        }

        public double SetLevel(OrganismMeasure measure, double level)
        {
           return this.measurementData.SetLevel(measure, level);
        }

        public double AdjustLevel(OrganismMeasure measure, double adjustment)
        {
            return this.measurementData.AdjustLevel(measure, adjustment);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} | {2} | {3} | {4}", this.Name, this.GetLevel(OrganismMeasure.Health).ToString("0.000"), this.Age.ToString("0.00"), this.Intention, this.Color);
        }
    }
}