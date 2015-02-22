namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;

    public interface IEcosystem
    {
        int Width { get; }

        int Height { get; }

        EcosystemRates EcosystemRates { get; }

        IWeather Weather { get; }

        PhaseSummary ExecuteOnePhase();
    }
}