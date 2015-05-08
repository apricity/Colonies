namespace Wacton.Colonies.Domain.Organisms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Intentions;
    using Wacton.Colonies.Domain.Measures;

    public class Organism : IOrganism
    {
        public Guid ColonyId { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }

        private IOrganismLogic OrganismLogic { get; set; }
        public string Description
        {
            get
            {
                return this.OrganismLogic.Description;
            }
        }

        public double Age { get; private set; }

        private Intention currentIntention;
        public Intention CurrentIntention
        {
            get
            {
                return this.IsAlive ? this.currentIntention : Intention.Dead;
            }
            private set
            {
                this.currentIntention = value;
            }
        }

        private double CurrentIntentionStartAge { get; set; }

        public Inventory CurrentInventory { get; private set; }

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
                return this.GetLevel(OrganismMeasure.Health) > 0.0;
            }
        }

        public bool CanMove
        {
            get
            {
                return !this.CurrentIntention.Equals(Intention.Reproduce);
            }
        }

        public bool IsAudible
        {
            get
            {
                return this.IsSoundOverloaded || this.IsSounding(this);
            }
        }

        public bool IsDepositingPheromone
        {
            get
            {
                // TODO: intention duration limit should probably be relative to the size of the ecosystem
                return this.IsPheromoneOverloaded || (this.CurrentIntention.Equals(Intention.Nourish) && this.IsWithinDuration(this.CurrentIntentionStartAge, 50));
            }
        }

        public Dictionary<EnvironmentMeasure, double> MeasureBiases
        {
            get
            {
                return this.CurrentIntention.EnvironmentBias;
            }
        }

        // TODO: overload and disease durations should come from settings
        private double PheromoneOverloadStartAge { get; set; }
        public bool IsPheromoneOverloaded
        {
            get
            {
                return this.IsWithinDuration(this.PheromoneOverloadStartAge, 20);
            }
        }

        private double SoundOverloadedStartAge { get; set; }
        public bool IsSoundOverloaded
        {
            get
            {
                return this.IsWithinDuration(this.SoundOverloadedStartAge, 20);
            }
        }

        private double DiseaseStartAge { get; set; }
        public bool IsDiseased
        {
            get
            {
                return this.IsWithinDuration(this.DiseaseStartAge, 100);
            }
        }

        public bool IsInfectious
        {
            get
            {
                return this.IsWithinDuration(this.DiseaseStartAge, 10);
            }
        }

        public Organism(Guid colonyId, string name, Color color, IOrganismLogic organismLogic)
        {
            this.ColonyId = colonyId;
            this.Name = name;
            this.Color = color;
            this.OrganismLogic = organismLogic;

            var health = new Measurement<OrganismMeasure>(OrganismMeasure.Health, 1.0);
            var inventory = new Measurement<OrganismMeasure>(OrganismMeasure.Inventory, 0.0);
            this.measurementData = new MeasurementData<OrganismMeasure>(new List<Measurement<OrganismMeasure>> { health, inventory });
            this.CurrentIntention = Intention.None;
            this.CurrentInventory = this.OrganismLogic.PreferredInventory;

            this.PheromoneOverloadStartAge = double.NaN;
            this.SoundOverloadedStartAge = double.NaN;
            this.DiseaseStartAge = double.NaN;
        }

        public void IncrementAge(double increment)
        {
            this.Age += increment;
        }

        private bool IsSounding(IOrganismState organismState)
        {
            return this.OrganismLogic.IsSounding(organismState);
        }

        public Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return this.OrganismLogic.DecideIntention(measurableEnvironment, this);
        }

        public void UpdateIntention(Intention newIntention)
        {
            if (this.CurrentIntention.Equals(newIntention))
            {
                return;
            }

            if (newIntention.HasConflictingInventory(this.CurrentIntention))
            {
                this.SetLevel(OrganismMeasure.Inventory, 0.0);
                this.CurrentInventory = newIntention.AssociatedInventory;
            }

            this.CurrentIntention = newIntention;
            this.CurrentIntentionStartAge = this.Age;
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

        public void OverloadPheromone()
        {
            if (!this.IsPheromoneOverloaded)
            {
                this.PheromoneOverloadStartAge = this.Age;
            }
        }

        public void OverloadSound()
        {
            if (!this.IsSoundOverloaded)
            {
                this.SoundOverloadedStartAge = this.Age;
            }
        }

        public void ContractDisease()
        {
            if (!this.IsDiseased)
            {
                this.DiseaseStartAge = this.Age;
            }
        }

        public bool CanAct(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return this.CurrentIntention.CanInteractEnvironment(measurableEnvironment, this);
        }

        public IntentionAdjustments ActionEffects(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            return this.CurrentIntention.InteractEnvironmentAdjustments(measurableEnvironment, this);
        }

        public bool CanInteract()
        {
            return this.CurrentIntention.CanInteractOrganism(this);
        }

        public IntentionAdjustments InteractionEffects(IOrganismState otherOrganismState)
        {
            return this.CurrentIntention.InteractOrganismAdjustments(this, otherOrganismState);
        }

        private bool IsWithinDuration(double startAge, double duration)
        {
            return !double.IsNaN(startAge) && this.GetDuration(startAge) <= duration;
        }

        private double GetDuration(double startAge)
        {
            return Math.Round(this.Age - startAge, 4);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} | {2} | {3} | {4} | {5}", this.Name, this.GetLevel(OrganismMeasure.Health).ToString("0.000"), this.Age.ToString("0.00"), this.Description, this.CurrentIntention, this.Color);
        }
    }
}