namespace Colonies.Models
{
    public class HabitatCondition
    {
        // TODO: each type of strength (e.g. pheromone, heat, moisture) should be between 0.0 - 1.0
        // TODO: the organism will assign a multiplier to that value (could change according to organism type / genetics)
        // TODO: that value will be added to the weighting of that habitat
        public readonly int Strength;

        public HabitatCondition(int strength)
        {
            this.Strength = strength;
        }
    }
}
