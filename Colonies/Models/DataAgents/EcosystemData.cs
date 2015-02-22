namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemData
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Habitat> OrganismHabitats { get; set; }
        private Dictionary<Habitat, Coordinate> HabitatCoordinates { get; set; }
        private IEcosystemHistoryPusher EcosystemHistoryPusher { get; set; }

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

        public EcosystemData(Habitat[,] habitats, Dictionary<Organism, Coordinate> organismCoordinates, IEcosystemHistoryPusher ecosystemHistory)
        {
            this.Habitats = habitats;
            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            this.OrganismHabitats = new Dictionary<Organism, Habitat>();
            this.EcosystemHistoryPusher = ecosystemHistory;

            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinate(i, j));
                }
            }

            foreach (var organismCoordinate in organismCoordinates)
            {
                var organism = organismCoordinate.Key;
                var coordinate = organismCoordinate.Value;

                this.AddOrganism(organism, this.HabitatAt(coordinate));
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
            return this.OrganismHabitats.Keys.Where(organism => organism.CurrentIntention.Equals(intention)).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> AliveOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> DeadOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => !organism.IsAlive).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> DepositingPheromoneOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive && organism.IsDepositingPheromone).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> AudibleOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive && organism.IsAudible).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> DiseasedOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive && organism.IsDiseased).Select(this.CoordinateOf);
        }

        public IEnumerable<Coordinate> InfectiousOrganismCoordinates()
        {
            return this.OrganismHabitats.Keys.Where(organism => organism.IsAlive && organism.IsInfectious).Select(this.CoordinateOf);
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

        public void AdjustLevel(Coordinate coordinate, EnvironmentMeasure measure, double adjustment)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).AdjustLevel(measure, adjustment);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void AdjustLevel(Coordinate coordinate, OrganismMeasure measure, double adjustment)
        {
            var previousLevel = this.GetLevel(coordinate, measure);
            var updatedLevel = this.HabitatAt(coordinate).AdjustLevel(measure, adjustment);
            this.RecordHistory(coordinate, measure, previousLevel, updatedLevel);
        }

        public void AdjustLevels(IEnumerable<Coordinate> coordinates, EnvironmentMeasure measure, double adjustment)
        {
            foreach (var coordinate in coordinates)
            {
                this.AdjustLevel(coordinate, measure, adjustment);
            }
        }

        public void AdjustLevels(IEnumerable<Coordinate> coordinates, OrganismMeasure measure, double adjustment)
        {
            foreach (var coordinate in coordinates)
            {
                this.AdjustLevel(coordinate, measure, adjustment);
            }
        }

        public void AdjustLevels(Coordinate coordinate, IntentionAdjustments intentionAdjustments)
        {
            foreach (var organismMeasureAdjustment in intentionAdjustments.OrganismMeasureAdjustments)
            {
                this.AdjustLevel(coordinate, organismMeasureAdjustment.Key, organismMeasureAdjustment.Value);
            }

            foreach (var organismMeasureAdjustment in intentionAdjustments.EnvironmentMeasureAdjustments)
            {
                this.AdjustLevel(coordinate, organismMeasureAdjustment.Key, organismMeasureAdjustment.Value);
            }
        }

        private void RecordHistory(Coordinate coordinate, IMeasure measure, double previousLevel, double updatedLevel)
        {
            this.EcosystemHistoryPusher.Push(new EcosystemModification(coordinate, measure, previousLevel, updatedLevel));
        }

        public bool IsHarmful(Coordinate coordinate)
        {
            return this.HabitatAt(coordinate).Environment.IsHarmful;
        }

        public void AddOrganism(IOrganism organism, Coordinate coordinate)
        {
            this.AddOrganism((Organism)organism, this.HabitatAt(coordinate));
            this.EcosystemHistoryPusher.Push(new EcosystemAddition(organism, coordinate));
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
            this.EcosystemHistoryPusher.Push(new EcosystemRelocation(organism, this.CoordinateOf(sourceHabitat), this.CoordinateOf(destinationHabitat)));
        }

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

        public void RefreshOrganismIntentions()
        {
            foreach (var organismCoordinate in this.AliveOrganismCoordinates())
            {
                var organism = this.GetOrganism(organismCoordinate);
                var environment = this.GetEnvironment(organismCoordinate);
                var intention = organism.DecideIntention(environment);
                organism.UpdateIntention(intention);
            }
        }

        public void IncrementOrganismAges(double increment)
        {
            foreach (var organismCoordinate in this.AliveOrganismCoordinates())
            {
                var organism = this.GetOrganism(organismCoordinate);
                organism.IncrementAge(increment);
            }
        }
    }
}