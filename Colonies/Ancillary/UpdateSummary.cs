namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;

    public class UpdateSummary
    {
        // TODO: might be overengineering for now, could just list all coordinates that have been affected
        public List<Coordinate> PreUpdateOrganismLocations { get; private set; }
        public List<Coordinate> PostUpdateOrganismLocations { get; private set; }
        public List<Coordinate> PheromoneDecreasedLocations { get; private set; }
        public List<Coordinate> NutrientGrowthLocations { get; private set; }
        public List<Coordinate> ObstructionDemolishLocations { get; private set; } 

        public UpdateSummary(
            List<Coordinate> preUpdateOrganismLocations, 
            List<Coordinate> postUpdateOrganismLocations,
            List<Coordinate> pheromoneDecreasedLocations,
            List<Coordinate> nutrientGrowthLocations,
            List<Coordinate> obstructionDemolishLocations)
        {
            this.PreUpdateOrganismLocations = preUpdateOrganismLocations;
            this.PostUpdateOrganismLocations = postUpdateOrganismLocations;
            this.PheromoneDecreasedLocations = pheromoneDecreasedLocations;
            this.NutrientGrowthLocations = nutrientGrowthLocations;
            this.ObstructionDemolishLocations = obstructionDemolishLocations;
        }

        public override string ToString()
        {
            return string.Format("Pre: {0}, Post: {1}", this.PreUpdateOrganismLocations.Count, this.PostUpdateOrganismLocations.Count);
        }
    }
}
