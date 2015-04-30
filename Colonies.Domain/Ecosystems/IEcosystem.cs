namespace Wacton.Colonies.Domain.Ecosystems
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Weathers;

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