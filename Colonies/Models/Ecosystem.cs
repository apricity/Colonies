namespace Wacton.Colonies.Models
{
    using System;
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
        private EcosystemStages EcosystemStages { get; set; }

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
        public Ecosystem(EcosystemData ecosystemData, EcosystemRates ecosystemRates, IEcosystemHistoryPuller ecosystemHistoryPuller, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor, EcosystemStages ecosystemStages)
        {
            this.EcosystemData = ecosystemData;
            this.EcosystemRates = ecosystemRates;
            this.EcosystemHistoryPuller = ecosystemHistoryPuller;
            this.Weather = weather;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;
            this.EcosystemStages = ecosystemStages;
        }

        public UpdateSummary UpdateOneStage()
        {
            var updateNumber = this.EcosystemStages.UpdateCount + 1; // because ecosystem stages is zero-based
            var updatesPerTurn = this.EcosystemStages.StageCount;
            this.EcosystemStages.ExecuteStage();
            var ecosystemHistory = this.EcosystemHistoryPuller.Pull();
            var updateSummary = new UpdateSummary(updateNumber, updatesPerTurn, ecosystemHistory, this.EcosystemData.OrganismCoordinatePairs());
            return updateSummary;
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