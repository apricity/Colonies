namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class Ecosystem
    {
        // TODO: blitz the current notion of "Habitat"
        public List<List<Habitat>> Habitats { get; set; }

        // TODO: probably want something like...
        // TODO: also wrap this bloody List<List<>> so it can be handled more neatly?
        public List<List<Environment>> Environments { get; set; }
        public List<Organism> organisms { get; set; } 
        // note that the above things are the things that Habitat used to know about
        private Dictionary<Organism, Point> OrganismLocations { get; set; }
        // since the dictionary knows about the organisms anyway, probably remove the List<Organism> and just have...
        private Dictionary<Organism, Point> Organisms { get; set; }
        // organism's environment: 
        // var currentEnvironment = this.Environments[this.Organisms[currentOrganism].Value.X][this.Organisms[currentOrganism].Value.y];
        // we'll tidy it up!

        public int Height
        {
            get
            {
                return this.Habitats.First().Count;
            }
        }

        public int Width
        {
            get
            {
                return this.Habitats.Count;
            }
        }

        public Ecosystem(List<List<Habitat>> habitats)
        {
            this.Habitats = habitats;
        }

        public void Update()
        {
            // organisms are now the things that make the decisions about where to move
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action
            var habitatsAndLocalAreas = this.GetLocalAreasOfOrganisms();
            foreach (var habitatAndLocalArea in habitatsAndLocalAreas)
            {
                var currentHabitat = habitatAndLocalArea.Key;
                var currentLocalArea = habitatAndLocalArea.Value;
                
                var destinationHabitat = currentHabitat.Organism.TakeTurn(currentLocalArea);
                if (!destinationHabitat.ContainsOrganism())
                {
                    this.MoveOrganism(currentHabitat, destinationHabitat);
                }
            }
        }

        private void MoveOrganism(Habitat sourceHabitat, Habitat destinationHabitat)
        {
            if (!sourceHabitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Source habitat {0} does not contain an organism", sourceHabitat), "sourceHabitat");
            }

            if (destinationHabitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Destination habitat {0} already contains an organism", destinationHabitat), "destinationHabitat");
            }

            var organismToMove = sourceHabitat.Organism;
            this.RemoveOrganism(sourceHabitat);
            this.AddOrganism(destinationHabitat, organismToMove);
        }

        private void AddOrganism(Habitat habitat, Organism organism)
        {
            if (habitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Organism already exists at habitat {0}", habitat), "habitat");
            }

            habitat.Organism = organism;
        }

        private void RemoveOrganism(Habitat habitat)
        {
            if (!habitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("No organism exists at habitat {0}", habitat), "habitat");
            }

            habitat.Organism = null;
        }


        // TODO: holy hells, this needs some thought...
        private Dictionary<Habitat, List<Habitat>> GetLocalAreasOfOrganisms()
        {
            var localAreaOfOrganisms = new Dictionary<Habitat, List<Habitat>>();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Habitats[x][y].ContainsOrganism())
                    {
                        var currentHabitat = this.Habitats[x][y];

                        // TODO [Waff]: refactor this into its own method
                        var localAreaOfHabitat = new List<Habitat>();
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            if (i < 0 || i >= this.Width)
                            {
                                continue;
                            }
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (j < 0 || j >= this.Height)
                                {
                                    continue;
                                }

                                localAreaOfHabitat.Add(Habitats[i][j]);
                            }
                        }
                        var localArea = new LocalArea(localAreaOfHabitat, this.Habitats[x][y]);
                        localAreaOfOrganisms.Add(currentHabitat, localArea.LocalHabitats);
                    }
                }
            }

            return localAreaOfOrganisms;
        }

        private IEnumerable<LocalArea> GetLocalAreaOfOccupiedHabitats()
        {
            var localAreaOfOccupiedHabitats = new List<LocalArea>();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Habitats[x][y].ContainsOrganism())
                    {
                        // TODO [Waff]: refactor this into its own method
                        var localAreaOfHabitat = new List<Habitat>();
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            if (i < 0 || i >= this.Width)
                            {
                                continue;
                            }
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (j < 0 || j >= this.Height)
                                {
                                    continue;
                                }

                                localAreaOfHabitat.Add(Habitats[i][j]);
                            }
                        }
                        var localArea = new LocalArea(localAreaOfHabitat,this.Habitats[x][y]);
                        localAreaOfOccupiedHabitats.Add(localArea);
                    }
                }
            }

            return localAreaOfOccupiedHabitats;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }
    }
}
