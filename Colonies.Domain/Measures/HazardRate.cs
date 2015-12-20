namespace Wacton.Colonies.Domain.Measures
{
    public class HazardRate
    {
        public double AddRate { get; }
        public double SpreadRate { get; }
        public double RemoveRate { get; }

        public HazardRate(double addRate, double spreadRate, double removeRate)
        {
            this.AddRate = addRate;
            this.SpreadRate = spreadRate;
            this.RemoveRate = removeRate;
        }

        public override string ToString() => $"Add {this.AddRate} | Spread {this.SpreadRate} | Remove {this.RemoveRate}";
    }
}
