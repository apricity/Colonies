namespace Wacton.Colonies.Models
{
    using System;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

    public class Queen : Organism
    {
        public Queen(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Nest;
        }

        public override double ProcessNutrient(double availableNutrient)
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

        public override double ProcessMineral(double availableMineral)
        {
            if (!availableMineral.Equals(1.0) || this.Intention.Equals(Intention.Eat))
            {
                return 0;
            }

            if (this.Intention.Equals(Intention.Nest))
            {
                this.Intention = Intention.Reproduce;
                return 0;
            }

            if (this.Intention.Equals(Intention.Reproduce) && this.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                this.Intention = Intention.Nest;
                return availableMineral;
            }

            return 0;
        }

        protected override void RefreshIntention()
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.25)
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
