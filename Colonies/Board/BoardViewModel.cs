namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    using Colonies.Annotations;

    public sealed class BoardViewModel : INotifyPropertyChanged
    {
        private readonly Timer modelTimer;

        // TODO: remove these, quick test variables
        private bool secondOrganismAdded = false;
        private bool leftToRightMovementDirection = true;

        private List<List<Tile>> tiles;
        public List<List<Tile>> Tiles
        {
            get
            {
                return this.tiles;
            }
            set
            {
                this.tiles = value;
                this.OnPropertyChanged("Tiles");
            }
        }

        private List<Organism> Organisms { get; set; }

        private IEnumerable<Tile> ListOfOccupiedTiles
        {
            get
            {
                var result = new List<Tile>();
                foreach (var tileRow in this.Tiles)
                {
                    foreach (var tile in tileRow)
                    {
                        if (tile.ContainsOrganism)
                        {
                            result.Add(tile);
                        }
                    } 
                }

                return result;
            }
        }

        public BoardViewModel()
        {
            this.InitialiseModel();

            TimerCallback modelTimerCallback = this.UpdateModel;
            this.modelTimer = new Timer(modelTimerCallback, null, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        private void InitialiseModel()
        {
            this.Tiles = new List<List<Tile>>();
            this.Organisms = new List<Organism>();

            var random = new Random();

            for (var column = 0; column < Properties.Settings.Default.BoardWidth; column++)
            {
                var tileColumn = new List<Tile>();
                for (var row = 0; row < Properties.Settings.Default.BoardHeight; row++)
                {
                    Terrain terrain;

                    var randomNumber = random.Next(0, 4);
                    switch (column)
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

                    tileColumn.Add(new Tile(column, row, terrain));
                }

                this.Tiles.Add(tileColumn);
            }

            var occupant = new Organism("testOrganism");
            this.Tiles[1][1].Organism = occupant;
        }

        private void UpdateModel(object state)
        {
            // TODO: don't actually do anything this stupid...
            // TODO: how should I handle updating the model?

            foreach (var tile in this.ListOfOccupiedTiles)
            {
                // temporarily store the tile Organism
                var tileOccupant = tile.Organism;
                
                // remove Organism from this tile
                // and move it to a different tile
                tile.Organism = null;

                if (this.leftToRightMovementDirection)
                {
                    if (tile.X + 1 >= this.Tiles.Count)
                    {
                        this.leftToRightMovementDirection = false;
                        this.Tiles[tile.X - 1][tile.Y].Organism = tileOccupant;
                    }
                    else
                    {
                        this.Tiles[tile.X + 1][tile.Y].Organism = tileOccupant;
                    }
                }
                else
                {
                    if (tile.X - 1 < 0)
                    {
                        this.leftToRightMovementDirection = true;
                        this.Tiles[tile.X + 1][tile.Y].Organism = tileOccupant;
                    }
                    else
                    {
                        this.Tiles[tile.X - 1][tile.Y].Organism = tileOccupant;
                    }
                }

                
            }

            if (!this.secondOrganismAdded)
            {
                this.Tiles[0][0].Organism = new Organism("anotherOrganism");
                this.secondOrganismAdded = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
