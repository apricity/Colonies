namespace Wacton.Colonies.Domain.Organisms
{
    using Wacton.Tovarisch.Enum;

    public class Inventory : Enumeration
    {
        public static readonly Inventory None = new Inventory("None");
        public static readonly Inventory Nutrient = new Inventory("Nutrient");
        public static readonly Inventory Mineral = new Inventory("Mineral");
        public static readonly Inventory Spawn = new Inventory("Spawn");

        private static int counter;

        private Inventory(string friendlyString)
            : base(counter++, friendlyString)
        {
        }
    }
}
