namespace Colonies.Models
{
    public class HabitatCondition
    {
        // TODO: each type of strength (e.g. pheromone, heat, moisture) should be between 0.0 - 1.0
        // TODO: the organism will assign a multiplier to that value (could change according to organism type / genetics)
        // TODO: that value will be added to the weighting of that habitat
        public readonly double Strength;

        public HabitatCondition(double strength)
        {
            this.Strength = strength;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Strength);
        }
    }
}
