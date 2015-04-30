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
    using Wacton.Colonies.Domain.Weathers;

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public EcosystemRates EcosystemRates { get; private set; }
        private IEcosystemHistoryPuller EcosystemHistoryPuller { get; set; }
        public IWeather Weather { get; private set; }
        public Distributor Distributor { get; private set; }
        private EcosystemPhases EcosystemPhases { get; set; }

        private IEnumerable<Coordinate> previousAudibleOrganismCoordinates;

        public int Width
        {
            get
            {
                return this.EcosystemData.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.EcosystemData.Height;
            }
        }

        // TODO: param list still a bit too long?
        public Ecosystem(EcosystemData ecosystemData, EcosystemRates ecosystemRates, IEcosystemHistoryPuller ecosystemHistoryPuller, IWeather weather, Distributor distributor, EcosystemPhases ecosystemPhases)
        {
            this.EcosystemData = ecosystemData;
            this.EcosystemRates = ecosystemRates;
            this.EcosystemHistoryPuller = ecosystemHistoryPuller;
            this.Weather = weather;
            this.Distributor = distributor;
            this.EcosystemPhases = ecosystemPhases;

            this.previousAudibleOrganismCoordinates = new List<Coordinate>();
        }

        public IHabitat HabitatAt(Coordinate coordinate)
        {
            return this.EcosystemData.GetHabitat(coordinate);
        }

        public PhaseSummary ExecuteOnePhase()
        {
            var phaseNumber = this.EcosystemPhases.PhaseCount + 1; // because ecosystem phases is zero-based
            var phasesPerRound = this.EcosystemPhases.PhasesPerRound;

            this.EcosystemPhases.ExecutePhase();
            this.EcosystemData.IncrementOrganismAges(1 / (double)phasesPerRound);
            this.ProcessChangedAfflictions();

            var ecosystemHistory = this.EcosystemHistoryPuller.Pull();
            var phaseSummary = new PhaseSummary(phaseNumber, phasesPerRound, ecosystemHistory, this.EcosystemData.OrganismCoordinatePairs());
            return phaseSummary;
        }

        // due to affliction timings in the organism, affliction can be present in the previous phase but not the current phase
        // (e.g. infectious at end of ambient phase, but not infectious at start of setup phase)
        // since each phase is independent, afflictions are being processed at this level instead of performing this for every phase
        private void ProcessChangedAfflictions()
        {
            var audibleOrganismCoordinates = this.EcosystemData.AudibleOrganismCoordinates().ToList();
            var infectiousAudibleOrganismCoordinates = this.EcosystemData.InfectiousOrganismCoordinates().ToList();
            this.Distributor.Remove(EnvironmentMeasure.Sound, this.previousAudibleOrganismCoordinates.ToList());
            this.Distributor.Insert(EnvironmentMeasure.Sound, audibleOrganismCoordinates);
            this.Distributor.Insert(EnvironmentMeasure.Disease, infectiousAudibleOrganismCoordinates);

            this.previousAudibleOrganismCoordinates = audibleOrganismCoordinates;
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure environmentMeasure, double level)
        {
            this.EcosystemData.SetLevel(coordinate, environmentMeasure, level);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.EcosystemData.OrganismCoordinates().Count());
        }
    }
}