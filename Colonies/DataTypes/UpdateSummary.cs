namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class UpdateSummary
    {
        public int ReferenceNumber { get; private set; }
        public IEcosystemHistory EcosystemHistory { get; private set; }
        public Dictionary<IOrganism, Coordinate> OrganismCoordinates { get; private set; } 

        public UpdateSummary(
            int updateReferenceNumber,
            IEcosystemHistory ecosystemHistory,
            Dictionary<IOrganism, Coordinate> organismCoordinates)
        {
            this.ReferenceNumber = updateReferenceNumber;
            this.EcosystemHistory = ecosystemHistory;
            this.OrganismCoordinates = organismCoordinates;
        }

        public override string ToString()
        {
            return string.Format(
                "# ref: {0} | # modifications: {1} | # relocations: {2}",
                this.ReferenceNumber,
                this.EcosystemHistory.Modifications.Count,
                this.EcosystemHistory.Relocations.Count);
        }
    }
}
