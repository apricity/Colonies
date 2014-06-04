namespace Wacton.Colonies.Ancillary
{
    using Wacton.Colonies.Interfaces;

    public class OrganismMeasure : Enumeration, IMeasure
    {
        public static readonly OrganismMeasure Health = new OrganismMeasure(0, "Health");

        private OrganismMeasure(int value, string friendlyString)
            : base(value, friendlyString)
        {
        }
    }
}
