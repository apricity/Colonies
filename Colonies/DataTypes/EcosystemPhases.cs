namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemPhases
    {
        private readonly List<IEcosystemPhase> ecosystemPhases;

        public int PhasesPerRound
        {
            get
            {
                return this.ecosystemPhases.Count;
            }
        }

        public int PhaseCount { get; private set; }

        public EcosystemPhases(List<IEcosystemPhase> ecosystemPhases)
        {
            this.ecosystemPhases = ecosystemPhases;
            this.PhaseCount = 0;
        }

        public void ExecutePhase()
        {
            var phaseIndex = this.PhaseCount % this.PhasesPerRound;
            this.ecosystemPhases[phaseIndex].Execute();
            this.PhaseCount++;
        }
    }
}
