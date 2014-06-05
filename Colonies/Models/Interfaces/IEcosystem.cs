namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public interface IEcosystem : IBiased<OrganismMeasure>
    {
        double HealthDeteriorationRate { get; set; }

        double PheromoneDepositRate { get; set; }

        double PheromoneFadeRate { get; set; }

        double NutrientGrowthRate { get; set; }

        double MineralFormRate { get; set; }

        double ObstructionDemolishRate { get; set; }

        IWeather Weather { get; }

        HazardChance GetHazardChance(EnvironmentMeasure hazardMeasure);

        void SetHazardChance(EnvironmentMeasure hazardMeasure, HazardChance hazardChance);

        UpdateSummary Update();
    }
}