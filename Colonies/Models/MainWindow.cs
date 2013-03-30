namespace Colonies.Models
{
    using System.Drawing;

    public class MainWindow
    {
        public Ecosystem Ecosystem { get; private set; }

        public MainWindow(Ecosystem ecosystem)
        {
            this.Ecosystem = ecosystem;
            this.InitialiseTerrain();
            this.InitialiseOrganisms();
        }

        private void InitialiseTerrain()
        {
            // apply a terrain for every habitat
            for (var x = 0; x < this.Ecosystem.Width; x++)
            {
                for (var y = 0; y < this.Ecosystem.Height; y++)
                {
                    Terrain terrain;

                    switch (x)
                    {
                        case 0:
                            terrain = Terrain.Earth;
                            break;
                        case 1:
                            terrain = Terrain.Grass;
                            break;
                        case 2:
                            terrain = Terrain.Water;
                            break;
                        case 3:
                            terrain = Terrain.Fire;
                            break;
                        default:
                            terrain = Terrain.Unknown;
                            break;
                    }

                    this.Ecosystem.Habitats[x][y].Environment.Terrain = terrain;
                }
            }
        }

        private void InitialiseOrganisms()
        {
            // place some organisms in the ecosystem
            // nothing clever yet, just removing all organisms and adding one in a starting position
            for (var x = 0; x < this.Ecosystem.Width; x++)
            {
                for (var y = 0; y < this.Ecosystem.Height; y++)
                {
                    this.Ecosystem.Habitats[x][y].Organism = null;
                }
            }

            this.Ecosystem.Habitats[0][0].Organism = new Organism("Waffle", Color.White);
            this.Ecosystem.Habitats[1][1].Organism = new Organism("Wacton", Color.Black);
        }
    }
}
