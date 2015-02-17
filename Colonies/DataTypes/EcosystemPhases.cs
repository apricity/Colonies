namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemPhases
    {
        private readonly List<IEcosystemPhase> ecosystemPhases;

        public int PhaseCount
        {
            get
            {
                return this.ecosystemPhases.Count;
            }
        }

        public int UpdateCount { get; private set; }

        public EcosystemPhases(List<IEcosystemPhase> ecosystemPhases)
        {
            this.ecosystemPhases = ecosystemPhases;
            this.UpdateCount = 0;
        }

        public void ExecutePhase()
        {
            var phaseIndex = this.UpdateCount % this.PhaseCount;
            this.ecosystemPhases[phaseIndex].Execute();
            this.UpdateCount++;
        }
    }
}
