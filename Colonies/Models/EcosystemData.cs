namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Logic;

    public class EcosystemData
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Habitat> OrganismHabitats { get; set; }
        private Dictionary<Habitat, Coordinate> HabitatCoordinates { get; set; }
        private Dictionary<Measure, List<Coordinate>> HazardCoordinates { get; set; }
        // TODO: need NutrientCoordinates?

        public int Width
        {
            get
            {
                return this.Habitats.Width();
            }
        }

        public int Height
        {
            get
            {
                return this.Habitats.Height();
            }
        }

        public EcosystemData(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats)
        {
            this.Habitats = habitats;
            this.OrganismHabitats = organismHabitats;
            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            this.HazardCoordinates = new Dictionary<Measure, List<Coordinate>>();

            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinate(i, j));
                }
            }

            foreach (var hazardMeasure in Environment.HazardMeasures())
            {
                this.HazardCoordinates.Add(hazardMeasure, new List<Coordinate>());
            }
        }

        // TODO: use interfaces?
        public IEnumerable<Organism> GetOrganisms()
        {
            return this.OrganismHabitats.Keys.ToList();
        }

        public IEnumerable<Habitat> GetHabitats()
        {
            return this.Habitats.ToList();
        }

        public Habitat HabitatAt(Coordinate coordinate)
        {
            return this.Habitats[coordinate.X, coordinate.Y];
        }

        public Habitat HabitatOf(Organism organism)
        {
            return this.OrganismHabitats[organism];
        }

        public Coordinate CoordinateOf(Habitat habitat)
        {
            return this.HabitatCoordinates[habitat];
        }

        public Coordinate CoordinateOf(Organism organism)
        {
            return this.HabitatCoordinates[this.OrganismHabitats[organism]];
        }

        public void AddOrganism(Organism organism, Coordinate coordinate)
        {
            this.AddOrganism(organism, this.HabitatAt(coordinate));
        }

        public void AddOrganism(Organism organism, Habitat habitat)
        {
            habitat.AddOrganism(organism);
            this.OrganismHabitats.Add(organism, habitat);
        }

        public void RemoveOrganism(Organism organism)
        {
            this.HabitatOf(organism).RemoveOrganism();
            this.OrganismHabitats.Remove(organism);
        }

        public void MoveOrganism(Organism organism, Habitat destinationHabitat)
        {
            var sourceHabitat = this.HabitatOf(organism);

            // the organism cannot move if it is dead
            if (!organism.IsAlive)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because it is dead",
                                   organism, destinationHabitat));
            }

            // the organism can only move to the destination if it is not obstructed
            if (destinationHabitat.IsObstructed())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is obstructed",
                                  organism, destinationHabitat));
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (destinationHabitat.ContainsOrganism())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is occupied by {2}",
                                  organism, destinationHabitat, destinationHabitat.Organism));
            }

            sourceHabitat.RemoveOrganism();
            destinationHabitat.AddOrganism(organism);
            this.OrganismHabitats[organism] = destinationHabitat;
        }

        public void InsertHazard(Measure hazardMeasure, Coordinate coordinate)
        {
            this.HazardCoordinates[hazardMeasure].Add(coordinate);
        }

        public IEnumerable<Coordinate> GetHazardCoordinates(Measure hazardMeasure)
        {
            return this.HazardCoordinates[hazardMeasure].ToList();
        }

        public Habitat[,] GetNeighbours(Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            return this.Habitats.GetNeighbours(coordinate, neighbourDepth, includeDiagonals, includeSelf);
        }
    }
}