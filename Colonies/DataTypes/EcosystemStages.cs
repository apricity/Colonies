namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemStages
    {
        private readonly List<IEcosystemStage> ecosystemStages;

        public int StageCount
        {
            get
            {
                return this.ecosystemStages.Count;
            }
        }

        public int UpdateCount { get; private set; }

        public EcosystemStages(List<IEcosystemStage> ecosystemStages)
        {
            this.ecosystemStages = ecosystemStages;
            this.UpdateCount = 0;
        }

        public void ExecuteStage()
        {
            var stageIndex = this.UpdateCount % this.StageCount;
            this.ecosystemStages[stageIndex].Execute();
            this.UpdateCount++;
        }
    }
}
