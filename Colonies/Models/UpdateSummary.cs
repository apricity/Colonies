namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;

    public class UpdateSummary
    {
        // TODO: might be overengineering for now, could just list all coordinates that have been affected
        public Dictionary<string, Coordinates> PreUpdateOrganismLocations { get; private set; }
        public Dictionary<string, Coordinates> PostUpdateOrganismLocations { get; private set; }
        public List<Coordinates> PheromoneDecreasedLocations { get; private set; } 

        public UpdateSummary(
            Dictionary<string, Coordinates> preUpdateOrganismLocations, 
            Dictionary<string, Coordinates> postUpdateOrganismLocations,
            List<Coordinates> pheromoneDecreasedLocations)
        {
            this.PreUpdateOrganismLocations = preUpdateOrganismLocations;
            this.PostUpdateOrganismLocations = postUpdateOrganismLocations;
            this.PheromoneDecreasedLocations = pheromoneDecreasedLocations;
        }

        public override string ToString()
        {
            return string.Format("Pre: {0}, Post: {1}", this.PreUpdateOrganismLocations.Count, this.PostUpdateOrganismLocations.Count);
        }
    }
}
