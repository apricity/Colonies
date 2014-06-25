namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;

    public class Queen : Organism
    {
        public Queen(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Nest;
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Nutrient, 0.0);
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

            if (!availableMineral.Equals(1.0) || this.Intention.Equals(Intention.Eat))
            {
                return mineralTaken;
            }

            if (this.Intention.Equals(Intention.Nest))
            {
                this.Intention = Intention.Reproduce;
                return mineralTaken;
            }

            if (this.Intention.Equals(Intention.Reproduce) && this.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                this.Intention = Intention.Nest;
                mineralTaken = availableMineral;
            }

            return mineralTaken;
        }

        protected override double ProcessHazards(IEnumerable<IMeasurement<EnvironmentMeasure>> presentHazardousMeasurements)
        {
            return 0;
        }

        protected override void RefreshIntention()
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.33)
            {
                this.Intention = Intention.Eat;
            }
            else if (this.Intention.Equals(Intention.Eat))
            {
                this.Intention = Intention.Nest;
            }
        }
    }
}
