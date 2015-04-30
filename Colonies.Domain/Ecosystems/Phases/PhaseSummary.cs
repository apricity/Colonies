﻿namespace Wacton.Colonies.Domain.Ecosystems.Phases
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Organisms;

    public class PhaseSummary
    {
        public int PhaseNumber { get; private set; }
        public int PhasesPerRound { get; private set; }
        public IEcosystemHistory EcosystemHistory { get; private set; }
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

        public override string ToString()
        {
            return string.Format(
                "# update: {0} | # modifications: {1} | # relocations: {2} | # additions: {3}",
                this.PhaseNumber,
                this.EcosystemHistory.Modifications.Count,
                this.EcosystemHistory.Relocations.Count,
                this.EcosystemHistory.Additions.Count);
        }
    }
}