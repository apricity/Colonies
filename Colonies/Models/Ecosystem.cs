namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class Ecosystem
    {
        // TODO: blitz the current notion of "Habitat"
        // public List<List<Habitat>> Habitats { get; set; }
        public Environment[,] Environments { get; set; }
        public Dictionary<Organism, Point> Organisms { get; set; }

        public int Width
        {
            get
            {
                return this.Environments.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return this.Environments.GetLength(1);
            }
        }
        
        public Ecosystem(Environment[,] environments, Dictionary<Organism, Point> organisms)
        {
            this.Environments = environments;
            this.Organisms = organisms;
        }

        public void Update()
        {
            var random = new Random();

            // organisms are now the things that make the decisions about where to move
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action
            foreach (var organism in Organisms.ToList())
            {
                var intendedDestination = organism.Key.TakeTurn(null);
                var randomPoint = new Point(random.Next(this.Width), random.Next(this.Height));

                // if (!this.ContainsOrganism(x, y))
                this.MoveOrganism(organism.Key, randomPoint);
            }
        }

        private void MoveOrganism(Organism organism, Point destination)
        {
            this.Organisms[organism] = destination;
        }

        public void AddOrganism(Organism organism, Point point)
        {
            this.Organisms.Add(organism, point);
        }

        private void RemoveOrganism(Organism organism)
        {
            this.Organisms.Remove(organism);
        }


        // TODO: holy hells, this needs some thought...
        //private Dictionary<Habitat, List<Habitat>> GetLocalAreasOfOrganisms()
        //{
        //    var localAreaOfOrganisms = new Dictionary<Habitat, List<Habitat>>();
        //    for (int x = 0; x < this.Width; x++)
        //    {
        //        for (int y = 0; y < this.Height; y++)
        //        {
        //            if (this.Habitats[x][y].ContainsOrganism())
        //            {
        //                var currentHabitat = this.Habitats[x][y];

        //                // TODO [Waff]: refactor this into its own method
        //                var localAreaOfHabitat = new List<Habitat>();
        //                for (int i = x - 1; i <= x + 1; i++)
        //                {
        //                    if (i < 0 || i >= this.Width)
        //                    {
        //                        continue;
        //                    }
        //                    for (int j = y - 1; j <= y + 1; j++)
        //                    {
        //                        if (j < 0 || j >= this.Height)
        //                        {
        //                            continue;
        //                        }

        //                        localAreaOfHabitat.Add(Habitats[i][j]);
        //                    }
        //                }
        //                var localArea = new LocalArea(localAreaOfHabitat, this.Habitats[x][y]);
        //                localAreaOfOrganisms.Add(currentHabitat, localArea.LocalHabitats);
        //            }
        //        }
        //    }

        //    return localAreaOfOrganisms;
        //}

        //private IEnumerable<LocalArea> GetLocalAreaOfOccupiedHabitats()
        //{
        //    var localAreaOfOccupiedHabitats = new List<LocalArea>();
        //    for (int x = 0; x < this.Width; x++)
        //    {
        //        for (int y = 0; y < this.Height; y++)
        //        {
        //            if (this.Habitats[x][y].ContainsOrganism())
        //            {
        //                // TODO [Waff]: refactor this into its own method
        //                var localAreaOfHabitat = new List<Habitat>();
        //                for (int i = x - 1; i <= x + 1; i++)
        //                {
        //                    if (i < 0 || i >= this.Width)
        //                    {
        //                        continue;
        //                    }
        //                    for (int j = y - 1; j <= y + 1; j++)
        //                    {
        //                        if (j < 0 || j >= this.Height)
        //                        {
        //                            continue;
        //                        }

        //                        localAreaOfHabitat.Add(Habitats[i][j]);
        //                    }
        //                }
        //                var localArea = new LocalArea(localAreaOfHabitat,this.Habitats[x][y]);
        //                localAreaOfOccupiedHabitats.Add(localArea);
        //            }
        //        }
        //    }

        //    return localAreaOfOccupiedHabitats;
        //}

        public override String ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }
    }
}
