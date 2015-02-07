namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class UpdateSummary
    {
        public int UpdateNumber { get; private set; }
        public int UpdatesPerTurn { get; private set; }
        public IEcosystemHistory EcosystemHistory { get; private set; }
        public Dictionary<IOrganism, Coordinate> OrganismCoordinates { get; private set; } 

        public UpdateSummary(
            int updateNumber,
            int updatesPerTurn,
            IEcosystemHistory ecosystemHistory,
            Dictionary<IOrganism, Coordinate> organismCoordinates)
        {
            this.UpdateNumber = updateNumber;
            this.UpdatesPerTurn = updatesPerTurn;
            this.EcosystemHistory = ecosystemHistory;
            this.OrganismCoordinates = organismCoordinates;
        }

        public override string ToString()
        {
            return string.Format(
                "# update: {0} | # modifications: {1} | # relocations: {2} | # additions: {3}",
                this.UpdateNumber,
                this.EcosystemHistory.Modifications.Count,
                this.EcosystemHistory.Relocations.Count,
                this.EcosystemHistory.Additions.Count);
        }
    }
}
