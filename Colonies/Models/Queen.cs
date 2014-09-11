namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public class Queen : Organism
    {
        public Queen(string name, Color color)
            : base(name, color)
        {
            // TODO: should be able to specify no inventory available?

            this.Intention = Intention.Nest;
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Nutrient, 0.0);
        }

        public override bool NeedsAssistance
        {
            get
            {
                return this.Intention.Equals(Intention.Reproduce) && this.GetLevel(OrganismMeasure.Health) <= 0.75;
            }
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

            if (this.Intention.Equals(Intention.Eat) || this.Intention.Equals(Intention.Nest))
            {
                return mineralTaken;
            }

            if (this.Intention.Equals(Intention.Reproduce) && this.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                mineralTaken = availableMineral;
            }

            return mineralTaken;
        }

        protected override double ProcessHazards(IEnumerable<IMeasurement<EnvironmentMeasure>> presentHazardousMeasurements)
        {
            return 0;
        }

        public override void RefreshIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.33)
            {
                this.Intention = Intention.Eat;
            }
            else
            {
                if (measurableEnvironment.MeasurementData.GetLevel(EnvironmentMeasure.Mineral) < 1.0)
                {
                    this.Intention = Intention.Nest;
                }
                else
                {
                    this.Intention = Intention.Reproduce;
                }
            }
        }
    }
}
