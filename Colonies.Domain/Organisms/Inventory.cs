namespace Wacton.Colonies.Domain.Organisms
{
    using Wacton.Tovarisch.Enum;

    public class Inventory : Enumeration
    {
        public static readonly Inventory None = new Inventory(0, "None");
        public static readonly Inventory Nutrient = new Inventory(1, "Nutrient");
        public static readonly Inventory Mineral = new Inventory(2, "Mineral");
        public static readonly Inventory Spawn = new Inventory(3, "Spawn");

        private Inventory(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }
    }
}
