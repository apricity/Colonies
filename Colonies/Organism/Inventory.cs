namespace Wacton.Colonies.Organism
{
    using Wacton.Tovarisch.Enum;

    public class Inventory : Enumeration
    {
        public static readonly Inventory Nutrient = new Inventory(0, "Nutrient");
        public static readonly Inventory Mineral = new Inventory(1, "Mineral");
        public static readonly Inventory Spawn = new Inventory(2, "Spawn");

        private Inventory(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }
    }
}
