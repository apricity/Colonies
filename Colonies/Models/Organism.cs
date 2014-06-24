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
        protected Intention Intention { get; set; }

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

        public Measurement<EnvironmentMeasure> Inventory { get; private set; }

        protected Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Measurement<OrganismMeasure>(OrganismMeasure.Health, 1.0);
            this.measurementData = new MeasurementData<OrganismMeasure>(new List<Measurement<OrganismMeasure>> { health });
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Nutrient, 0.0);
        }

        public abstract double ProcessNutrient(double availableNutrient);

        public abstract double ProcessMineral(double availableMineral);

        public Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            this.RefreshIntention();

            if (this.Intention.Equals(Intention.Eat))
            {
                // TODO: move to method
                if (this.Inventory.Measure.Equals(EnvironmentMeasure.Nutrient))
                {
                    var availableInventoryNutrient = this.Inventory.Level;
                    var desiredInventoryNutrient = 1 - this.GetLevel(OrganismMeasure.Health);
                    var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
                    this.IncreaseLevel(OrganismMeasure.Health, inventoryNutrientTaken);
                    this.Inventory.DecreaseLevel(inventoryNutrientTaken);
                }
            }

            var reducedMeasures = new Dictionary<EnvironmentMeasure, double>();

            var nutrientTaken = this.ProcessNutrient(measurableEnvironment.GetLevel(EnvironmentMeasure.Nutrient));
            if (nutrientTaken > 0.0)
            {
                reducedMeasures.Add(EnvironmentMeasure.Nutrient, nutrientTaken);
            }

            var mineralTaken = this.ProcessMineral(measurableEnvironment.GetLevel(EnvironmentMeasure.Mineral));
            if (mineralTaken > 0.0)
            {
                reducedMeasures.Add(EnvironmentMeasure.Mineral, mineralTaken);
            }

            return reducedMeasures;
        }

        protected abstract void RefreshIntention();

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