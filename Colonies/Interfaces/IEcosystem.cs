namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface IEcosystem : IBiased
    {
        double HealthDeteriorationRate { get; set; }

        double PheromoneDepositRate { get; set; }

        double PheromoneFadeRate { get; set; }

        double NutrientGrowthRate { get; set; }

        double MineralFormRate { get; set; }

        double ObstructionDemolishRate { get; set; }

        double HazardSpreadRate { get; set; }

        IWeather Weather { get; }

        UpdateSummary Update();
    }
}