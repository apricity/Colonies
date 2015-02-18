namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.DataAgents;
    using Wacton.Colonies.Models.Interfaces;

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public EcosystemRates EcosystemRates { get; private set; }
        private IEcosystemHistoryPuller EcosystemHistoryPuller { get; set; }
        public IWeather Weather { get; private set; }
        public EnvironmentMeasureDistributor EnvironmentMeasureDistributor { get; private set; }
        private EcosystemPhases EcosystemPhases { get; set; }

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
        public Ecosystem(EcosystemData ecosystemData, EcosystemRates ecosystemRates, IEcosystemHistoryPuller ecosystemHistoryPuller, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor, EcosystemPhases ecosystemPhases)
        {
            this.EcosystemData = ecosystemData;
            this.EcosystemRates = ecosystemRates;
            this.EcosystemHistoryPuller = ecosystemHistoryPuller;
            this.Weather = weather;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;
            this.EcosystemPhases = ecosystemPhases;
        }

        public UpdateSummary ExecuteOnePhase()
        {
            var updateNumber = this.EcosystemPhases.UpdateCount + 1; // because ecosystem phases is zero-based
            var updatesPerTurn = this.EcosystemPhases.PhaseCount;

            // TODO: review this now that intention is only updated once per turn - can do it at setup phase?
            var previousAudibleOrganismCoordinates = this.EcosystemData.AudibleOrganismCoordinates().ToList();

            this.EcosystemPhases.ExecutePhase();
            this.EcosystemData.IncrementOrganismAges(1 / (double)this.EcosystemPhases.PhaseCount);

            var currentAudibleOrganismCoordinates = this.EcosystemData.AudibleOrganismCoordinates().ToList();
            var infectiousOrganismCoordinates = this.EcosystemData.InfectiousOrganismCoordinates().ToList();

            this.RemoveEnvironmentDistribution(previousAudibleOrganismCoordinates, EnvironmentMeasure.Sound);
            this.InsertEnvironmentDistribution(currentAudibleOrganismCoordinates, EnvironmentMeasure.Sound);

            // TODO: should this just go into movement phase?
            this.InsertEnvironmentDistribution(infectiousOrganismCoordinates, EnvironmentMeasure.Disease);

            var ecosystemHistory = this.EcosystemHistoryPuller.Pull();
            var updateSummary = new UpdateSummary(updateNumber, updatesPerTurn, ecosystemHistory, this.EcosystemData.OrganismCoordinatePairs());
            return updateSummary;
        }

        private void RemoveEnvironmentDistribution(IEnumerable<Coordinate> coordinates, EnvironmentMeasure environmentMeasure)
        {
            foreach (var coordinate in coordinates)
            {
                this.EnvironmentMeasureDistributor.RemoveDistribution(coordinate, environmentMeasure);
            }
        }

        private void InsertEnvironmentDistribution(IEnumerable<Coordinate> coordinates, EnvironmentMeasure environmentMeasure)
        {
            foreach (var coordinate in coordinates)
            {
                this.EnvironmentMeasureDistributor.InsertDistribution(coordinate, environmentMeasure);
            }
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure environmentMeasure, double level)
        {
            this.EcosystemData.SetLevel(coordinate, environmentMeasure, level);
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.EcosystemData.OrganismCoordinates().Count());
        }
    }
}