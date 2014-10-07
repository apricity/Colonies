namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
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
