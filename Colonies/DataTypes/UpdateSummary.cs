namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Models.Interfaces;

    public class UpdateSummary
    {
        public Dictionary<IOrganism, Coordinate> PreviousOrganismCoordinates { get; set; }
        public Dictionary<IOrganism, Coordinate> CurrentOrganismCoordinates { get; set; }

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
            Dictionary<IOrganism, Coordinate> previousOrganismCoordinates,
            Dictionary<IOrganism, Coordinate> currentOrganismCoordinates,
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
