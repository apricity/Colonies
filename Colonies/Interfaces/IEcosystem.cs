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

        IWeather Weather { get; }

        HazardChance GetHazardChance(Measure hazardMeasure);

        void SetHazardChance(Measure hazardMeasure, HazardChance hazardChance);

        UpdateSummary Update();
    }
}