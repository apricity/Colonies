namespace Wacton.Colonies.Ancillary
{
    public class HazardChance
    {
        public double AddChance { get; private set; }
        public double SpreadChance { get; private set; }
        public double RemoveChance { get; private set; }

        public HazardChance(double addChance, double spreadChance, double removeChance)
        {
            this.AddChance = addChance;
            this.SpreadChance = spreadChance;
            this.RemoveChance = removeChance;
        }

        public override string ToString()
        {
            return string.Format("Add {0} | Spread {1} | Remove {2}", this.AddChance, this.SpreadChance, this.RemoveChance);
        }
    }
}
