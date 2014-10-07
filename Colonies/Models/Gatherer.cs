namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Gatherer : Organism
    {
        public Gatherer(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Harvest;
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Nutrient, 0.0);
        }

        public override bool NeedsAssistance
        {
            get
            {
                return false;
            }
        }

        public override void RefreshIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment)
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
