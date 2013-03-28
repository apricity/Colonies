namespace Colonies
{
    public sealed class Habitat
    {
        public Terrain Terrain { get; set; }

        public Habitat(Terrain terrain)
        {
            this.Terrain = terrain;
        }
        
        public override string ToString()
        {
            return this.Terrain.ToString();
        }
    }
}
