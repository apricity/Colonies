namespace Wacton.Colonies.DataTypes.Enums
{
    using Wacton.Colonies.DataTypes.Interfaces;

    public class OrganismMeasure : Enumeration, IMeasure
    {
        public static readonly OrganismMeasure Health = new OrganismMeasure(0, "Health");
        public static readonly OrganismMeasure Inventory = new OrganismMeasure(1, "Inventory");

        private OrganismMeasure(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }
    }
}
