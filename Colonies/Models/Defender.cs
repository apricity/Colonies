namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Defender : Organism
    {
        public Defender(string name, Color color)
            : base(name, color, Inventory.Mineral, Intention.Mine)
        {
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
            if (this.GetLevel(OrganismMeasure.Health) < 0.33)
            {
                this.UpdateIntention(Intention.Eat);
            }
            else
            {
                this.UpdateIntention(this.GetLevel(OrganismMeasure.Inventory) < 1.0 ? Intention.Mine : Intention.Build);
            }
        }
    }
}
