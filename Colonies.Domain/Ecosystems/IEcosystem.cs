namespace Wacton.Colonies.Domain.Ecosystems
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Settings;
    using Wacton.Colonies.Domain.Weathers;

    public interface IEcosystem
    {
        int Width { get; }

        int Height { get; }

        IEcosystemSettings EcosystemSettings { get; }

        IWeather Weather { get; }

        IHabitat HabitatAt(Coordinate coordinate);

        PhaseSummary ExecuteOnePhase();
    }
}