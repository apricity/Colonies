namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Defender : Organism
    {
        public Defender(string name, Color color)
            : base(name, color)
        {
            this.Intention = Intention.Mine;
            this.Inventory = new Measurement<EnvironmentMeasure>(EnvironmentMeasure.Mineral, 0.0);
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
                this.Intention = Intention.Eat;
            }
            else
            {
                this.Intention = this.Inventory.Level < 1.0 ? Intention.Mine : Intention.Build;
            }
        }
    }
}
