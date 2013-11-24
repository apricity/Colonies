namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Logic;

    public class EcosystemData
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Habitat> OrganismHabitats { get; set; }
        private Dictionary<Habitat, Coordinate> HabitatCoordinates { get; set; }
        private Dictionary<Measure, List<Coordinate>> HazardCoordinates { get; set; }

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

        public EcosystemData(Habitat[,] habitats, Dictionary<Organism, Coordinate> organismCoordinates)
        {
            this.Habitats = habitats;
            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            this.OrganismHabitats = new Dictionary<Organism, Habitat>();
            this.HazardCoordinates = new Dictionary<Measure, List<Coordinate>>();

            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinate(i, j));
                }
            }

            foreach (var organismCoordinate in organismCoordinates)
            {
                this.AddOrganism(organismCoordinate.Key, organismCoordinate.Value);
            }

            foreach (var hazardMeasure in Environment.HazardMeasures())
            {
                this.HazardCoordinates.Add(hazardMeasure, new List<Coordinate>());
            }
        }

        public IEnumerable<Organism> GetOrganisms()
        {
            return this.OrganismHabitats.Keys.ToList();
        }

        public IEnumerable<Coordinate> AllCoordinates()
        {
            return this.HabitatCoordinates.Values.ToList();
        }

        public IEnumerable<Coordinate> OrganismCoordinates(bool? isAlive, bool? isDepositingPheromone)
        {
            return this.OrganismHabitats.Keys.Where(organism => 
                isAlive == null || organism.IsAlive == isAlive 
                && isDepositingPheromone == null || organism.IsDepositingPheromones == isDepositingPheromone)
                .Select(this.CoordinateOf);
        }

        public bool HasMeasure(Coordinate coordinate, Measure measure)
        {
            if (Environment.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Environment.GetLevel(measure) > 0.0;
            }
            else if (Organism.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Organism.GetLevel(measure) > 0.0;
            }
            else
            {
                throw new ArgumentException("Measure was not found");
            }
        }

        public double GetLevel(Coordinate coordinate, Measure measure)
        {
            if (Environment.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Environment.GetLevel(measure);
            }
            else if (Organism.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Organism.GetLevel(measure);
            }
            else
            {
                throw new ArgumentException("Measure was not found");
            }
        }

        public void SetLevel(Coordinate coordinate, Measure measure, double level)
        {
            if (Environment.Measures().Contains(measure))
            {
                this.HabitatAt(coordinate).Environment.SetLevel(measure, level);
            }
            else if (Organism.Measures().Contains(measure))
            {
                this.HabitatAt(coordinate).Organism.SetLevel(measure, level);
            }
            else
            {
                throw new ArgumentException("Measure was not found"); 
            }
        }

        public bool IncreaseLevel(Coordinate coordinate, Measure measure, double increment)
        {
            if (Environment.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Environment.IncreaseLevel(measure, increment);
            }
            else if (Organism.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Organism.IncreaseLevel(measure, increment);
            }
            else
            {
                throw new ArgumentException("Measure was not found");
            }
        }

        public bool DecreaseLevel(Coordinate coordinate, Measure measure, double decrement)
        {
            if (Environment.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Environment.DecreaseLevel(measure, decrement); 
            }
            else if (Organism.Measures().Contains(measure))
            {
                return this.HabitatAt(coordinate).Organism.DecreaseLevel(measure, decrement); 
            }
            else
            {
                throw new ArgumentException("Measure was not found");
            }
        }

        public bool IsHazardous(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment.IsHazardous;
        }

        private Habitat HabitatAt(Coordinate coordinate)
        {
            return this.Habitats[coordinate.X, coordinate.Y];
        }

        private Habitat HabitatOf(Organism organism)
        {
            return this.OrganismHabitats[organism];
        }

        public Coordinate CoordinateOf(Habitat habitat)
        {
            return this.HabitatCoordinates[habitat];
        }

        public Coordinate CoordinateOf(Organism organism)
        {
            return this.CoordinateOf(this.OrganismHabitats[organism]);
        }

        private void AddOrganism(Organism organism, Coordinate coordinate)
        {
            this.AddOrganism(organism, this.HabitatAt(coordinate));
        }

        private void AddOrganism(Organism organism, Habitat habitat)
        {
            habitat.AddOrganism(organism);
            this.OrganismHabitats.Add(organism, habitat);
        }

        private void RemoveOrganism(Organism organism)
        {
            this.HabitatOf(organism).RemoveOrganism();
            this.OrganismHabitats.Remove(organism);
        }

        public void MoveOrganism(Organism organism, Coordinate coordinate)
        {
            this.MoveOrganism(organism, this.HabitatAt(coordinate));
        }

        private void MoveOrganism(Organism organism, Habitat destinationHabitat)
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

        public Coordinate[,] GetNeighbours(Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            return this.Habitats.GetNeighbours(coordinate, neighbourDepth, includeDiagonals, includeSelf);
        }

        public IMeasurable GetMeasurable(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment;
        }
    }
}