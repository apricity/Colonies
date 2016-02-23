namespace Wacton.Colonies.Domain.Measures
{
    using Wacton.Tovarisch.Enum;

    public class OrganismMeasure : Enumeration, IMeasure
    {
        public static readonly OrganismMeasure Health = new OrganismMeasure("Health");
        public static readonly OrganismMeasure Inventory = new OrganismMeasure("Inventory");

        private static int counter;

        private OrganismMeasure(string friendlyString)
            : base(counter++, friendlyString)
        {
        }
    }
}
