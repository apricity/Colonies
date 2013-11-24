namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Models;

    public class UpdateSummary
    {
        public Dictionary<IMeasurableOrganism, Coordinate> PreviousOrganismCoordinates { get; set; }
        public Dictionary<IMeasurableOrganism, Coordinate> CurrentOrganismCoordinates { get; set; }

        public List<Coordinate> AlteredEnvironmentCoordinates { get; private set; }

        public Dictionary<IMeasurableOrganism, Coordinate> ActiveOrganismPreviousCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.PreviousOrganismCoordinates.Except(this.CurrentOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<IMeasurableOrganism, Coordinate> ActiveOrganismCurrentCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.CurrentOrganismCoordinates.Except(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<IMeasurableOrganism, Coordinate> InactiveOrganismCoordinates
        {
            get
            {
                // an organism is inactive if its current coordinate is the same as previous
                return this.CurrentOrganismCoordinates.Intersect(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public UpdateSummary(
            Dictionary<IMeasurableOrganism, Coordinate> previousOrganismCoordinates,
            Dictionary<IMeasurableOrganism, Coordinate> currentOrganismCoordinates,
            List<Coordinate> alteredEnvironmentCoordinates)
        {
            this.PreviousOrganismCoordinates = previousOrganismCoordinates;
            this.CurrentOrganismCoordinates = currentOrganismCoordinates;
            this.AlteredEnvironmentCoordinates = alteredEnvironmentCoordinates;
        }

        public override string ToString()
        {
            return string.Format(
                "# previous organisms: {0} | # current organisms: {1} | # altered environments: {2}",
                this.PreviousOrganismCoordinates.Count,
                this.CurrentOrganismCoordinates.Count,
                this.AlteredEnvironmentCoordinates.Count);
        }
    }
}
