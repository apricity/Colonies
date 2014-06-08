namespace Wacton.Colonies.Models.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemData
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Habitat> OrganismHabitats { get; set; }
        private Dictionary<Habitat, Coordinate> HabitatCoordinates { get; set; }
        private Dictionary<EnvironmentMeasure, List<Coordinate>> HazardCoordinates { get; set; }

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
            this.HazardCoordinates = new Dictionary<EnvironmentMeasure, List<Coordinate>>();

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

            foreach (var environmentMeasure in EnvironmentMeasure.HazardousMeasures())
            {
                this.HazardCoordinates.Add(environmentMeasure, new List<Coordinate>());
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

        public IEnumerable<Coordinate> GetOrganismEmittingSoundCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsEmittingSound).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> GetHazardCoordinates(EnvironmentMeasure hazardMeasure)
        {
            return this.HazardCoordinates[hazardMeasure].ToList();
        }

        public bool HasLevel(Coordinate coordinate, EnvironmentMeasure measure)
        {
            return this.HabitatAt(coordinate).GetLevel(measure) > 0.0;
        }

        public bool HasLevel(Coordinate coordinate, OrganismMeasure measure)
        {
            return this.HabitatAt(coordinate).GetLevel(measure) > 0.0;
        }

        public double GetLevel(Coordinate coordinate, EnvironmentMeasure measure)
        {
            return this.HabitatAt(coordinate).GetLevel(measure);
        }

        public double GetLevel(Coordinate coordinate, OrganismMeasure measure)
        {
            return this.HabitatAt(coordinate).GetLevel(measure);
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure measure, double level)
        {
            this.HabitatAt(coordinate).SetLevel(measure, level);
        }

        public void SetLevel(Coordinate coordinate, OrganismMeasure measure, double level)
        {
            this.HabitatAt(coordinate).SetLevel(measure, level);
        }

        public bool IncreaseLevel(Coordinate coordinate, EnvironmentMeasure measure, double increment)
        {
            return this.HabitatAt(coordinate).IncreaseLevel(measure, increment);
        }

        public bool IncreaseLevel(Coordinate coordinate, OrganismMeasure measure, double increment)
        {
            return this.HabitatAt(coordinate).IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(Coordinate coordinate, EnvironmentMeasure measure, double decrement)
        {
            return this.HabitatAt(coordinate).DecreaseLevel(measure, decrement); 
        }

        public bool DecreaseLevel(Coordinate coordinate, OrganismMeasure measure, double decrement)
        {
            return this.HabitatAt(coordinate).DecreaseLevel(measure, decrement);
        }

        public void InsertHazard(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (!this.HazardCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardCoordinates[environmentMeasure].Add(coordinate);
            }
        }

        public void RemoveHazard(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (this.HazardCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardCoordinates[environmentMeasure].Remove(coordinate);
            }
        }

        public bool IsHarmful(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment.IsHarmful;
        }

        private void AddOrganism(Organism organism, Coordinate coordinate)
        {
            this.AddOrganism(organism, this.HabitatAt(coordinate));
        }

        private void AddOrganism(Organism organism, Habitat habitat)
        {
            habitat.SetOrganism(organism);
            this.OrganismHabitats.Add(organism, habitat);
        }

        private void RemoveOrganism(Organism organism)
        {
            this.HabitatOf(organism).ResetOrganism();
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

            sourceHabitat.ResetOrganism();
            destinationHabitat.SetOrganism(organism);
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
    }
}