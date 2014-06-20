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
        }

        public override double ProcessNutrient(double availableNutrient)
        {
            return 0;
        }

        public override double ProcessMineral(double availableMineral)
        {
            return 0;
        }

        protected override void RefreshIntention()
        {
        }
    }
}
