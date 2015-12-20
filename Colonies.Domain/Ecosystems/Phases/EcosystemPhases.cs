namespace Wacton.Colonies.Domain.Ecosystems.Phases
{
    using System.Collections.Generic;

    public class EcosystemPhases
    {
        private readonly List<IEcosystemPhase> ecosystemPhases;

        public int PhasesPerRound => this.ecosystemPhases.Count;
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
