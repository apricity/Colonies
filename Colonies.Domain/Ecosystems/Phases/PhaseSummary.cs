namespace Wacton.Colonies.Domain.Ecosystems.Phases
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Organisms;

    public class PhaseSummary
    {
        public int PhaseNumber { get; }
        public int PhasesPerRound { get; private set; }
        public IEcosystemHistory EcosystemHistory { get; }
        public Dictionary<IOrganism, Coordinate> OrganismCoordinates { get; private set; } 

        public PhaseSummary(
            int phaseNumber,
            int phaserPerRound,
            IEcosystemHistory ecosystemHistory,
            Dictionary<IOrganism, Coordinate> organismCoordinates)
        {
            this.PhaseNumber = phaseNumber;
            this.PhasesPerRound = phaserPerRound;
            this.EcosystemHistory = ecosystemHistory;
            this.OrganismCoordinates = organismCoordinates;
        }

        public override string ToString() => $"# update: {this.PhaseNumber} | # modifications: {this.EcosystemHistory.Modifications.Count} | # relocations: {this.EcosystemHistory.Relocations.Count} | # additions: {this.EcosystemHistory.Additions.Count}";
    }
}
