namespace Wacton.Colonies.DataTypes
{
    public class HazardRate
    {
        public double AddRate { get; private set; }
        public double SpreadRate { get; private set; }
        public double RemoveRate { get; private set; }

        public HazardRate(double addRate, double spreadRate, double removeRate)
        {
            this.AddRate = addRate;
            this.SpreadRate = spreadRate;
            this.RemoveRate = removeRate;
        }

        public override string ToString()
        {
            return string.Format("Add {0} | Spread {1} | Remove {2}", this.AddRate, this.SpreadRate, this.RemoveRate);
        }
    }
}
