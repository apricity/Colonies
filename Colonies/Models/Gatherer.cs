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

        protected override bool IsSounding()
        {
            return false;
        }

        public override Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.25)
            {
                return Intention.Eat;
            }

            return this.GetLevel(OrganismMeasure.Inventory) < 0.75 ? Intention.Harvest : Intention.Nourish;
        }
    }
}
