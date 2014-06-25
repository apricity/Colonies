namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;

    public class Defender : Organism
    {
        public Defender(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Mine;
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Mineral, 0.0);
        }

        protected override double ProcessNutrient(double availableNutrient)
        {
            var nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (this.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - this.GetLevel(OrganismMeasure.Health);
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.IncreaseLevel(OrganismMeasure.Health, nutrientTaken);
            }

            return nutrientTaken;
        }

        protected override double ProcessMineral(double availableMineral)
        {
            var mineralTaken = 0.0;

            if (availableMineral.Equals(0.0))
            {
                return mineralTaken;
            }

            if (this.Intention.Equals(Intention.Mine))
            {
                var desiredMineral = 1 - this.Inventory.Level;
                mineralTaken = Math.Min(desiredMineral, availableMineral);
                this.Inventory.IncreaseLevel(mineralTaken);
            }

            return mineralTaken;
        }

        protected override double ProcessHazards(IEnumerable<IMeasurement<EnvironmentMeasure>> presentHazardousMeasurements)
        {
            var obstructionCreated = 0.0;

            if (!this.Intention.Equals(Intention.Build) || this.Inventory.Level < 1.0)
            {
                return obstructionCreated;
            }

            if (presentHazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                this.Inventory.DecreaseLevel(1.0);
                obstructionCreated = 1.0;
            }

            return obstructionCreated;
        }

        protected override void RefreshIntention()
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.33)
            {
                this.Intention = Intention.Eat;
            }
            else
            {
                this.Intention = this.Inventory.Level < 1.0 ? Intention.Mine : Intention.Build;
            }
        }
    }
}
