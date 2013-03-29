namespace Colonies.Models
{
    public sealed class Environment
    {
        public Terrain Terrain { get; set; }

        public Environment(Terrain terrain)
        {
            this.Terrain = terrain;
        }
        
        public override string ToString()
        {
            return this.Terrain.ToString();
        }
    }
}
