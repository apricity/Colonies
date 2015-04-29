namespace Wacton.Colonies.Domain.Ecosystem
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystem.Data;
    using Wacton.Colonies.Domain.Ecosystem.Phases;
    using Wacton.Colonies.Domain.Habitat;
    using Wacton.Colonies.Domain.Weather;

    public interface IEcosystem
    {
        int Width { get; }

        int Height { get; }

        EcosystemRates EcosystemRates { get; }

        IWeather Weather { get; }

        IHabitat HabitatAt(Coordinate coordinate);

        PhaseSummary ExecuteOnePhase();
    }
}