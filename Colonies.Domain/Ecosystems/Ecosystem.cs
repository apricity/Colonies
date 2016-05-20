namespace Wacton.Colonies.Domain.Ecosystems
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Ecosystems.Modification;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Settings;
    using Wacton.Colonies.Domain.Weathers;

    public class Ecosystem : IEcosystem
    {
        private readonly EcosystemData ecosystemData;
        private readonly IEcosystemHistoryPuller ecosystemHistoryPuller;
        private readonly EcosystemPhases ecosystemPhases;

        public IEcosystemSettings EcosystemSettings { get; }
        public IWeather Weather { get; }
        public Distributor Distributor { get; }

        private IEnumerable<Coordinate> previousAudibleOrganismCoordinates;

        public int Width => this.ecosystemData.Width;
        public int Height => this.ecosystemData.Height;

        // TODO: param list still a bit too long?
        public Ecosystem(EcosystemData ecosystemData, EcosystemSettings ecosystemSettings, IEcosystemHistoryPuller ecosystemHistoryPuller, IWeather weather, Distributor distributor, EcosystemPhases ecosystemPhases)
        {
            this.ecosystemData = ecosystemData;
            this.EcosystemSettings = ecosystemSettings;
            this.ecosystemHistoryPuller = ecosystemHistoryPuller;
            this.Weather = weather;
            this.Distributor = distributor;
            this.ecosystemPhases = ecosystemPhases;

            this.previousAudibleOrganismCoordinates = new List<Coordinate>();
        }

        public IHabitat HabitatAt(Coordinate coordinate)
        {
            return this.ecosystemData.GetHabitat(coordinate);
        }

        public PhaseSummary ExecuteOnePhase()
        {
            var phaseNumber = this.ecosystemPhases.PhaseCount + 1; // because ecosystem phases is zero-based
            var phasesPerRound = this.ecosystemPhases.PhasesPerRound;

            this.ecosystemPhases.ExecutePhase();
            this.ecosystemData.IncrementOrganismAges(1 / (double)phasesPerRound);
            this.ProcessChangedAfflictions();

            var ecosystemHistory = this.ecosystemHistoryPuller.Pull();
            var phaseSummary = new PhaseSummary(phaseNumber, phasesPerRound, ecosystemHistory, this.ecosystemData.OrganismCoordinatePairs());
            return phaseSummary;
        }

        // due to affliction timings in the organism, affliction can be present in the previous phase but not the current phase
        // (e.g. infectious at end of ambient phase, but not infectious at start of setup phase)
        // since each phase is independent, afflictions are being processed at this level instead of performing this for every phase
        private void ProcessChangedAfflictions()
        {
            var audibleOrganismCoordinates = this.ecosystemData.AudibleOrganismCoordinates().ToList();
            var infectiousAudibleOrganismCoordinates = this.ecosystemData.InfectiousOrganismCoordinates().ToList();
            this.Distributor.Remove(EnvironmentMeasure.Sound, this.previousAudibleOrganismCoordinates.ToList());
            this.Distributor.Insert(EnvironmentMeasure.Sound, audibleOrganismCoordinates);
            this.Distributor.Insert(EnvironmentMeasure.Disease, infectiousAudibleOrganismCoordinates);

            this.previousAudibleOrganismCoordinates = audibleOrganismCoordinates;
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure environmentMeasure, double level)
        {
            this.ecosystemData.SetLevel(coordinate, environmentMeasure, level);
        }

        public override string ToString() => $"{this.Width}x{this.Height} : {this.ecosystemData.OrganismCoordinates().Count()} organisms";
    }
}