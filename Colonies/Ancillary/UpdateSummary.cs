namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Models;

    public class UpdateSummary
    {
        public Dictionary<Organism, Coordinate> PreviousOrganismCoordinates { get; set; }
        public Dictionary<Organism, Coordinate> CurrentOrganismCoordinates { get; set; }

        public List<Coordinate> AlteredEnvironmentCoordinates { get; private set; }

        public Dictionary<Organism, Coordinate> ActiveOrganismPreviousCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.PreviousOrganismCoordinates.Except(this.CurrentOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<Organism, Coordinate> ActiveOrganismCurrentCoordinates
        {
            get
            {
                // an organism is active if its current coordinate is different than previous
                return this.CurrentOrganismCoordinates.Except(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public Dictionary<Organism, Coordinate> InactiveOrganismCoordinates
        {
            get
            {
                // an organism is inactive if its current coordinate is the same as previous
                return this.CurrentOrganismCoordinates.Intersect(this.PreviousOrganismCoordinates)
                            .ToDictionary(organismCoordinate => organismCoordinate.Key, organismCoordinate => organismCoordinate.Value);
            }
        }

        public UpdateSummary(
            Dictionary<Organism, Coordinate> previousOrganismCoordinates, 
            Dictionary<Organism, Coordinate> currentOrganismCoordinates,
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
