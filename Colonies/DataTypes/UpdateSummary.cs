namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Models.Interfaces;

    public class UpdateSummary
    {
        public int ReferenceNumber { get; private set; }
        public Dictionary<IOrganism, Coordinate> PreviousOrganismCoordinates { get; private set; }
        public Dictionary<IOrganism, Coordinate> CurrentOrganismCoordinates { get; private set; }
        public List<Coordinate> AlteredEnvironmentCoordinates { get; private set; }

        public Dictionary<IOrganism, Coordinate> ActiveOrganismPreviousCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.PreviousOrganismCoordinates.Except(this.CurrentOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<IOrganism, Coordinate> ActiveOrganismCurrentCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.CurrentOrganismCoordinates.Except(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<IOrganism, Coordinate> InactiveOrganismCoordinates
        {
            get
            {
                // an organism is inactive if its current coordinate is the same as previous
                return this.CurrentOrganismCoordinates.Intersect(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public UpdateSummary(
            int updateReferenceNumber,
            Dictionary<IOrganism, Coordinate> previousOrganismCoordinates,
            Dictionary<IOrganism, Coordinate> currentOrganismCoordinates,
            List<Coordinate> alteredEnvironmentCoordinates)
        {
            this.ReferenceNumber = updateReferenceNumber;
            this.PreviousOrganismCoordinates = previousOrganismCoordinates;
            this.CurrentOrganismCoordinates = currentOrganismCoordinates;
            this.AlteredEnvironmentCoordinates = alteredEnvironmentCoordinates;
        }

        public override string ToString()
        {
            return string.Format(
                "# ref: {0} | # previous organisms: {1} | # current organisms: {2} | # altered environments: {3}",
                this.ReferenceNumber,
                this.PreviousOrganismCoordinates.Count,
                this.CurrentOrganismCoordinates.Count,
                this.AlteredEnvironmentCoordinates.Count);
        }
    }
}
