namespace Wacton.Colonies.Models
{
    using System;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

    public class Gatherer : Organism
    {
        public Gatherer(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Harvest;
        }

        public override double ProcessNutrient(double availableNutrient)
        {
            var nutrientTaken = 0.0;

            if (availableNutrient.Equals(0.0))
            {
                return nutrientTaken;
            }

            if (this.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - this.Inventory.Level;
                nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                this.Inventory.IncreaseLevel(nutrientTaken);
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
            return 0;
        }

        protected override void RefreshIntention()
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.25)
            {
                this.Intention = Intention.Eat;
            }
            else
            {
                this.Intention = this.Inventory.Level < 0.75 ? Intention.Harvest : Intention.Nourish;
            }
        }
    }
}
