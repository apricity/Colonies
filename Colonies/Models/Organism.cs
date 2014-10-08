﻿namespace Wacton.Colonies.Models
{
    using System;
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
        public Inventory Inventory { get; private set; }

        public Intention Intention { get; private set; }

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

        protected Organism(string name, Color color, Inventory inventoryType, Intention initialIntention)
        {
            this.Name = name;
            this.Color = color;
            this.Inventory = inventoryType;
            this.UpdateIntention(initialIntention);

            var health = new Measurement<OrganismMeasure>(OrganismMeasure.Health, 1.0);
            var inventory = new Measurement<OrganismMeasure>(OrganismMeasure.Inventory, 0.0);
            this.measurementData = new MeasurementData<OrganismMeasure>(new List<Measurement<OrganismMeasure>> { health, inventory });
        }

        public Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return OrganismLogic.ProcessMeasurableEnvironment(this, measurableEnvironment);
        }

        public void UpdateIntention(Intention intention)
        {
            if (intention.RequiresInventory && !intention.RequiredInventory.Equals(this.Inventory))
            {
                throw new InvalidOperationException(
                    string.Format("Intention {0} requires inventory {1}", intention, intention.RequiredInventory));
            }

            this.Intention = intention;
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