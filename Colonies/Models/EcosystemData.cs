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

        public IEnumerable<Coordinate> GetAllCoordinates()
        {
            return this.HabitatCoordinates.Values.ToList();
        }

        public IEnumerable<Coordinate> GetOrganismCoordinates(bool? isAlive, bool? isDepositingPheromone)
        {
            return this.OrganismHabitats.Keys.Where(organism => 
                isAlive == null || organism.IsAlive == isAlive 
                && isDepositingPheromone == null || organism.IsDepositingPheromones == isDepositingPheromone)
                .Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> GetHazardCoordinates(Measure hazardMeasure)
        {
            return this.HazardCoordinates[hazardMeasure].ToList();
        }

        public bool HasEnvironmentMeasure(Coordinate coordinate, Measure measure)
        {
            EnsureEnvironmentMeasure(measure);
            return this.HabitatAt(coordinate).Environment.GetLevel(measure) > 0.0;
        }

        public bool HasOrganismMeasure(Coordinate coordinate, Measure measure)
        {
            EnsureOrganismMeasure(measure);
            return this.HabitatAt(coordinate).Organism.GetLevel(measure) > 0.0;
        }

        public double GetEnvironmentLevel(Coordinate coordinate, Measure measure)
        {
            EnsureEnvironmentMeasure(measure);
            return this.HabitatAt(coordinate).Environment.GetLevel(measure);
        }

        public double GetOrganismLevel(Coordinate coordinate, Measure measure)
        {
            EnsureOrganismMeasure(measure);
            return this.HabitatAt(coordinate).Organism.GetLevel(measure);
        }

        public void SetEnvironmentLevel(Coordinate coordinate, Measure measure, double level)
        {
            EnsureEnvironmentMeasure(measure);
            this.HabitatAt(coordinate).Environment.SetLevel(measure, level);
        }

        public void SetOrganismLevel(Coordinate coordinate, Measure measure, double level)
        {
            EnsureOrganismMeasure(measure);
            this.HabitatAt(coordinate).Organism.SetLevel(measure, level);
        }

        public bool IncreaseEnvironmentLevel(Coordinate coordinate, Measure measure, double increment)
        {
            EnsureEnvironmentMeasure(measure);
            return this.HabitatAt(coordinate).Environment.IncreaseLevel(measure, increment);
        }

        public bool IncreaseOrganismLevel(Coordinate coordinate, Measure measure, double increment)
        {
            EnsureOrganismMeasure(measure);
            return this.HabitatAt(coordinate).Organism.IncreaseLevel(measure, increment);
        }

        public bool DecreaseEnvironmentLevel(Coordinate coordinate, Measure measure, double decrement)
        {
            EnsureEnvironmentMeasure(measure);
            return this.HabitatAt(coordinate).Environment.DecreaseLevel(measure, decrement); 
        }

        public bool DecreaseOrganismLevel(Coordinate coordinate, Measure measure, double decrement)
        {
            EnsureOrganismMeasure(measure);
            return this.HabitatAt(coordinate).Organism.DecreaseLevel(measure, decrement);
        }

        public void InsertHazard(Measure hazardMeasure, Coordinate coordinate)
        {
            this.HazardCoordinates[hazardMeasure].Add(coordinate);
        }

        public bool IsHazardous(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment.IsHazardous;
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

        public void MoveOrganism(IOrganism organism, Coordinate desiredHabitatCoordinate)
        {
            this.MoveOrganism((Organism)organism, this.HabitatAt(desiredHabitatCoordinate));
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

        public Coordinate[,] GetNeighbours(Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            return this.Habitats.GetNeighbours(coordinate, neighbourDepth, includeDiagonals, includeSelf);
        }

        public IEnvironment GetEnvironment(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment;
        }

        public IOrganism GetOrganism(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Organism;
        }

        private Habitat HabitatAt(Coordinate coordinate)
        {
            return this.Habitats[coordinate.X, coordinate.Y];
        }

        private Habitat HabitatOf(Organism organism)
        {
            return this.OrganismHabitats[organism];
        }

        private Coordinate CoordinateOf(Habitat habitat)
        {
            return this.HabitatCoordinates[habitat];
        }

        public Coordinate CoordinateOf(IOrganism organism)
        {
            return this.CoordinateOf((Organism)organism);
        }

        private Coordinate CoordinateOf(Organism organism)
        {
            return this.CoordinateOf(this.OrganismHabitats[organism]);
        }

        private static void EnsureEnvironmentMeasure(Measure measure)
        {
            if (!Environment.Measures().Contains(measure))
            {
                throw new ArgumentException("Measure argument is not an environment measure");
            }
        }

        private static void EnsureOrganismMeasure(Measure measure)
        {
            if (!Organism.Measures().Contains(measure))
            {
                throw new ArgumentException("Measure argument is not an organism measure");
            }
        }
    }
}