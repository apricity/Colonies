namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Logic;
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
                return this.IsAlive && this.Intention.Equals(Intention.Feed);
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

        public Inventory Inventory { get; private set; }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;

            var health = new Measurement(OrganismMeasure.Health, 1.0);
            this.measurementData = new MeasurementData(new List<Measurement> { health });
            this.Intention = Intention.Harvest;

            //var measure = DecisionLogic.MakeDecision(EnvironmentMeasure.TransportableMeasures());
            this.Inventory = new Inventory(EnvironmentMeasure.Nutrient, 0.0);
        }

        public void RefreshIntention()
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.25)
            {
                this.Intention = Intention.Eat;
            }
            else
            {
                this.Intention = this.Inventory.Amount < 0.75 ? Intention.Harvest : Intention.Feed;
            }
        }

        public double ProcessNutrient(double availableNutrient)
        {
            double nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (this.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - this.Inventory.Amount;
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.Inventory = new Inventory(EnvironmentMeasure.Nutrient, this.Inventory.Amount + nutrientTaken);
            }

            if (this.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - this.GetLevel(OrganismMeasure.Health);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.IncreaseLevel(OrganismMeasure.Health, nutrientTaken);
            }

            return nutrientTaken;
        }

        public double ProcessMineral(double availableMineral)
        {
            return 0;
        }

        public Dictionary<EnvironmentMeasure, double> PerformIntentionAction(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            this.RefreshIntention();

            if (this.Intention.Equals(Intention.Eat))
            {
                // TODO: move to method
                if (this.Inventory.EnvironmentMeasure.Equals(EnvironmentMeasure.Nutrient))
                {
                    var availableInventoryNutrient = this.Inventory.Amount;
                    var desiredInventoryNutrient = 1 - this.GetLevel(OrganismMeasure.Health);
                    var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);
                    this.IncreaseLevel(OrganismMeasure.Health, inventoryNutrientTaken);

                    // TODO: make inventory easier to use!
                    var remainingAmount = availableInventoryNutrient - inventoryNutrientTaken;
                    this.Inventory = new Inventory(EnvironmentMeasure.Nutrient, remainingAmount);
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

            //this.RefreshIntention();

            return reducedMeasures;
        }

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

        // TODO: these should be based on intentions and organism type
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

        public override string ToString()
        {
            return string.Format("{0}: {1} {2} {3}", this.Name, this.GetLevel(OrganismMeasure.Health), this.Intention, this.Color);
        }
    }
}