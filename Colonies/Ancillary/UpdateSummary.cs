namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;

    public class UpdateSummary
    {
        // TODO: might be overengineering for now, could just list all coordinates that have been affected
        public Dictionary<string, Coordinates> PreUpdateOrganismLocations { get; private set; }
        public Dictionary<string, Coordinates> PostUpdateOrganismLocations { get; private set; }
        public List<Coordinates> PheromoneDecreasedLocations { get; private set; }
        public List<Coordinates> NutrientGrowthLocations { get; private set; }
        public List<Coordinates> MineralGrowthLocations { get; private set; }

        public UpdateSummary(
            Dictionary<string, Coordinates> preUpdateOrganismLocations, 
            Dictionary<string, Coordinates> postUpdateOrganismLocations,
            List<Coordinates> pheromoneDecreasedLocations,
            List<Coordinates> nutrientGrowthLocations,
            List<Coordinates> mineralGrowthLocations)
        {
            this.PreUpdateOrganismLocations = preUpdateOrganismLocations;
            this.PostUpdateOrganismLocations = postUpdateOrganismLocations;
            this.PheromoneDecreasedLocations = pheromoneDecreasedLocations;
            this.NutrientGrowthLocations = nutrientGrowthLocations;
            this.MineralGrowthLocations = mineralGrowthLocations;
        }

        public override string ToString()
        {
            return string.Format("Pre: {0}, Post: {1}", this.PreUpdateOrganismLocations.Count, this.PostUpdateOrganismLocations.Count);
        }
    }
}
