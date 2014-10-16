namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.DataProviders;
    using Wacton.Colonies.Models.Interfaces;

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public EcosystemRates EcosystemRates { get; private set; }
        private IEcosystemHistoryPuller EcosystemHistoryPuller { get; set; }
        public IWeather Weather { get; private set; }
        public EnvironmentMeasureDistributor EnvironmentMeasureDistributor { get; private set; }

        // TODO: perhaps encapsulate within an "EcosystemStages" wrapper 
        private EnvironmentInteraction EnvironmentInteraction { get; set; }
        private OrganismMovement OrganismMovement { get; set; }
        private OrganismInteraction OrganismInteraction { get; set; }
        private EcosystemAdjustment EcosystemAdjustment { get; set; }

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

        private readonly List<IEcosystemStage> ecosystemStages;
        public int UpdateStages { get { return this.ecosystemStages.Count; } }
        private int updateCount = 0;

        // TODO: parameter list is now a bit too long after refactoring...
        public Ecosystem(EcosystemData ecosystemData, EcosystemRates ecosystemRates, IEcosystemHistoryPuller ecosystemHistory, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor, EnvironmentInteraction environmentInteraction, OrganismMovement organismMovement, OrganismInteraction organismInteraction, EcosystemAdjustment ecosystemAdjustment)
        {
            this.EcosystemData = ecosystemData;
            this.EcosystemRates = ecosystemRates;
            this.EcosystemHistoryPuller = ecosystemHistory;
            this.Weather = weather;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;
            this.EnvironmentInteraction = environmentInteraction;
            this.OrganismMovement = organismMovement;
            this.OrganismInteraction = organismInteraction;
            this.EcosystemAdjustment = ecosystemAdjustment;

            this.ecosystemStages = new List<IEcosystemStage>
                                       {
                                           this.EnvironmentInteraction,
                                           this.OrganismMovement,
                                           this.OrganismInteraction,
                                           this.EcosystemAdjustment
                                       };
        }

        public UpdateSummary UpdateOneStage()
        {
            var updateStageIndex = this.updateCount % this.UpdateStages;
            this.ecosystemStages[updateStageIndex].Execute();
            var ecosystemHistory = this.EcosystemHistoryPuller.Pull();
            var updateSummary = new UpdateSummary(this.updateCount, ecosystemHistory, this.EcosystemData.OrganismCoordinatePairs());
            this.updateCount++;
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