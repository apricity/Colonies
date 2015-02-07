namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Gatherer : Organism
    {
        public Gatherer(string name, Color color)
            : base(name, color, Inventory.Nutrient, Intention.Harvest)
        {
        }

        public override bool IsCalling
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
                this.UpdateIntention(Intention.Eat);
            }
            else
            {
                this.UpdateIntention(this.GetLevel(OrganismMeasure.Inventory) < 0.75 ? Intention.Harvest : Intention.Nourish);
            }
        }
    }
}
