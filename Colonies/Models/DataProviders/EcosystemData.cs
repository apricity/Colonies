namespace Wacton.Colonies.Models.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemData
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Habitat> OrganismHabitats { get; set; }
        private Dictionary<Habitat, Coordinate> HabitatCoordinates { get; set; }
        private Dictionary<EnvironmentMeasure, List<Coordinate>> HazardSourceCoordinates { get; set; }
        private IEcosystemHistoryPushOnly EcosystemHistory { get; set; }

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

        public EcosystemData(Habitat[,] habitats, Dictionary<Organism, Coordinate> organismCoordinates, IEcosystemHistoryPushOnly ecosystemHistory)
        {
            this.Habitats = habitats;
            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            this.OrganismHabitats = new Dictionary<Organism, Habitat>();
            this.HazardSourceCoordinates = new Dictionary<EnvironmentMeasure, List<Coordinate>>();
            this.EcosystemHistory = ecosystemHistory;

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
                this.HazardSourceCoordinates.Add(environmentMeasure, new List<Coordinate>());
            }
        }

        public Dictionary<IOrganism, Coordinate> OrganismCoordinatePairs()
        {
            return this.OrganismCoordinates().ToDictionary(this.GetOrganism, coordinate => coordinate);
        }

        public IEnumerable<Coordinate> AllCoordinates()
        {
            return this.HabitatCoordinates.Values.ToList();
        }

        public IEnumerable<Coordinate> OrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> OrganismCoordinates(Intention intention)
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.Intention.Equals(intention)).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> AliveOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> DeadOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => !organism.IsAlive).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> MoveableOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => !organism.Intention.Equals(Intention.Reproduce)).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> DepositingPheromoneOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsDepositingPheromone).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> EmittingSoundOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.Intention.Equals(Intention.Reproduce) && organism.NeedsAssistance).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> GetHazardSourceCoordinates(EnvironmentMeasure hazardMeasure)
        {
            return this.HazardSourceCoordinates[hazardMeasure].ToList();
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
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).SetLevel(measure, level);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void SetLevel(Coordinate coordinate, OrganismMeasure measure, double level)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).SetLevel(measure, level);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void IncreaseLevel(Coordinate coordinate, EnvironmentMeasure measure, double increment)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).IncreaseLevel(measure, increment);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void IncreaseLevel(Coordinate coordinate, OrganismMeasure measure, double increment)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).IncreaseLevel(measure, increment);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void DecreaseLevel(Coordinate coordinate, EnvironmentMeasure measure, double decrement)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).DecreaseLevel(measure, decrement);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void DecreaseLevel(Coordinate coordinate, OrganismMeasure measure, double decrement)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).DecreaseLevel(measure, decrement);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        private void RecordHistory(Coordinate coordinate, IMeasure measure, double previousLevel, double updatedLevel)
        {
            this.EcosystemHistory.Record(new EcosystemModification(coordinate, measure, previousLevel, updatedLevel));
        }

        // TODO: can this be used?
        //private void CheckHazardStatus(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
        //{
        //    if (!environmentMeasure.IsHazardous)
        //    {
        //        return;
        //    }

        //    var hazardLevel = this.GetLevel(coordinate, environmentMeasure);
        //    if (hazardLevel.Equals(1.0))
        //    {
        //        this.InsertHazardSource(environmentMeasure, coordinate);
        //    }
        //    else
        //    {
        //        this.RemoveHazardSource(environmentMeasure, coordinate);
        //    }
        //}

        public void InsertHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (!this.HazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardSourceCoordinates[environmentMeasure].Add(coordinate);
            }
        }

        public void RemoveHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (this.HazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardSourceCoordinates[environmentMeasure].Remove(coordinate);
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

        //public Coordinate[,] GetNeighbours(Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        //{
        //    return this.GetNeighbours(coordinate, neighbourDepth, includeDiagonals, includeSelf);
        //}

        public IEnumerable<Coordinate> GetValidNeighbours(Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            var neighbourCoordinates = this.GetNeighbours(coordinate, neighbourDepth, includeDiagonals, includeSelf).ToList();
            return neighbourCoordinates.Where(neighbourCoordinate => neighbourCoordinate != null);
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