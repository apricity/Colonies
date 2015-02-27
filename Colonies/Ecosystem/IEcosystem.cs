namespace Wacton.Colonies.Ecosystem
{
    using Wacton.Colonies.Ecosystem.Data;
    using Wacton.Colonies.Ecosystem.Phases;
    using Wacton.Colonies.Weather;

    public interface IEcosystem
    {
        int Width { get; }

        int Height { get; }

        EcosystemRates EcosystemRates { get; }

        IWeather Weather { get; }

        PhaseSummary ExecuteOnePhase();
    }
}