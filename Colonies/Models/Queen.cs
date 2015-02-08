namespace Wacton.Colonies.Models
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class Queen : Organism
    {
        public Queen(string name, Color color)
            : base(name, color, Inventory.Spawn, Intention.Nest)
        {
        }

        protected override bool IsSounding()
        {
            return this.Intention.Equals(Intention.Reproduce) && !this.IsReproductive;
        }

        public override Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment)
        {
            if (this.GetLevel(OrganismMeasure.Health) < 0.33)
            {
                return Intention.Eat;
            }

            if (this.GetLevel(OrganismMeasure.Inventory).Equals(1.0))
            {
                return Intention.Birth;
            }

            return measurableEnvironment.MeasurementData.GetLevel(EnvironmentMeasure.Mineral) < 1.0 ? Intention.Nest : Intention.Reproduce;
        }
    }
}
