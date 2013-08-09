﻿namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Logic;

    public class Ecosystem : IBiased
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Habitat> OrganismHabitats { get; private set; }
        public Dictionary<Habitat, Coordinates> HabitatCoordinates { get; private set; } 
        public double HealthBias { get; private set; }

        // TODO: neater management of these?
        public double PheromoneDepositPerTurn { get; set; }
        public double PheromoneFadePerTurn { get; set; }

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats)
        {
            this.Habitats = habitats;
            this.OrganismHabitats = organismHabitats;

            this.HealthBias = 1;

            this.HabitatCoordinates = new Dictionary<Habitat, Coordinates>();
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinates(i, j));
                }
            }

            this.PheromoneDepositPerTurn = 0.01;
            this.PheromoneFadePerTurn = 0.001;
        }

        public int Width
        {
            get
            {
                return this.Habitats.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return this.Habitats.GetLength(1);
            }
        }

        public UpdateSummary Update()
        {
            /* record the pre-update locations */
            var preUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key.ToString(), 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            /* reduce pheromone level in all environments 
             * and increase pheromone wherever appropriate */
            var pheromoneDecreasedLocations = this.DecreaseGlobalPheromoneLevel();
            this.IncreaseLocalPheromoneLevels();

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to 
             * and to resolve any conflicting intentions */
            var intendedOrganismDestinations = this.GetIntendedOrganismDestinations();
            var actualOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, new List<Organism>());

            /* perform in-situ actions e.g. take food, eat food, attack */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                var organism = actualOrganismDestination.Key;
                var habitat = actualOrganismDestination.Value;

                if (habitat.Environment.HasNutrient)
                {
                    organism.IncreaseHealth(1.0);
                    //habitat.Environment.HasNutrient = false; // this will remove the nutrient from the ecosystem
                }
            }

            /* perform ex-situ actions e.g. move any organisms that can after resolving conflicting intentions */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                this.MoveOrganism(actualOrganismDestination.Key, actualOrganismDestination.Value);                    
            }

            /* reduce all organisms health */
            this.DecreaseAllOrganismHealth();

            /* record the post-update locations */
            var postUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key.ToString(), 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            return new UpdateSummary(preUpdateOrganismLocations, postUpdateOrganismLocations, pheromoneDecreasedLocations);
        }

        private List<Coordinates> DecreaseGlobalPheromoneLevel()
        {
            var pheromoneDecreasedLocations = new List<Coordinates>();

            foreach (var habitat in this.Habitats)
            {
                if (habitat.Environment.DecreasePheromoneLevel(this.PheromoneFadePerTurn))
                {
                    pheromoneDecreasedLocations.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return pheromoneDecreasedLocations;
        }

        private void IncreaseLocalPheromoneLevels()
        {
            foreach (var organismHabitat in this.OrganismHabitats)
            {
                var organism = organismHabitat.Key;
                var habitat = organismHabitat.Value;

                if (organism.IsDepositingPheromones)
                {
                    habitat.Environment.IncreasePheromoneLevel(this.PheromoneDepositPerTurn);
                }
            }
        }

        private void DecreaseAllOrganismHealth()
        {
            foreach (var organism in this.OrganismHabitats.Keys.ToList())
            {
                organism.DecreaseHealth(0.005);
            }
        }

        protected virtual Dictionary<Organism, Habitat> GetIntendedOrganismDestinations()
        {
            var intendedOrganismDestinations = new Dictionary<Organism, Habitat>();
            var aliveOrganismHabitats = this.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
            foreach (var organismCoordinates in aliveOrganismHabitats)
            {
                var organism = organismCoordinates.Key;
                var habitat = organismCoordinates.Value;

                // get measurements of neighbouring environments
                var neighbouringHabitats = this.GetNeighbouringHabitats(habitat);

                // determine organism's intentions based on the measurements
                var chosenHabitat = DecisionLogic.MakeDecision(neighbouringHabitats, organism);
                intendedOrganismDestinations.Add(organism, chosenHabitat);
            }

            return intendedOrganismDestinations;
        }

        private Dictionary<Organism, Habitat> ResolveOrganismDestinations(Dictionary<Organism, Habitat> intendedOrganismDestinations, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismDestinations = new Dictionary<Organism, Habitat>();

            // create a copy of the organism habitats because we don't want to modify the actual set
            var currentOrganismHabitats = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key,
                organismHabitat => organismHabitat.Value);

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                currentOrganismHabitats.Remove(alreadyResolvedOrganism);
            }

            var occupiedHabitats = currentOrganismHabitats.Values.ToList();
            var intendedHabitats = intendedOrganismDestinations.Values.ToList();

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantHabitats = intendedHabitats.Except(occupiedHabitats).Where(habitat => !habitat.IsObstructed()).ToList();
            if (vacantHabitats.Count == 0)
            {
                return resolvedOrganismDestinations;
            }

            foreach (var habitat in vacantHabitats)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantHabitat = habitat;
                var conflictingOrganisms = intendedOrganismDestinations
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantHabitat))
                    .Select(intendedOrganismDestination => intendedOrganismDestination.Key)
                    .ToList();

                Organism organismToMove;
                if (conflictingOrganisms.Count > 1)
                {
                    organismToMove = this.DecideOrganism(conflictingOrganisms);
                    conflictingOrganisms.Remove(organismToMove);

                    // the remaining conflicting organisms cannot move, so reset their intended destinations
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        intendedOrganismDestinations[remainingOrganism] = this.OrganismHabitats[remainingOrganism];
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }
                
                // intended movement becomes an actual, resolved movement
                resolvedOrganismDestinations.Add(organismToMove, intendedOrganismDestinations[organismToMove]);
                intendedOrganismDestinations.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismDestinations.Keys.ToList();
            var trailingOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, resolvedOrganisms);
            foreach (var trailingOrganismDestination in trailingOrganismDestinations)
            {
                resolvedOrganismDestinations.Add(trailingOrganismDestination.Key, trailingOrganismDestination.Value);
            }

            return resolvedOrganismDestinations;
        }

        protected virtual Organism DecideOrganism(List<Organism> organisms)
        {
            // this is in a virtual method so the mock ecosystem can override for testing
            return DecisionLogic.MakeDecision(organisms, this);
        }

        private void MoveOrganism(Organism organism, Habitat destination)
        {           
            var source = this.OrganismHabitats[organism];

            // the organism cannot move if it is dead
            if (!organism.IsAlive)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because it is dead",
                                   organism, destination));
            }

            // the organism can only move to the destination if it is not obstructed
            if (destination.IsObstructed())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is obstructed", 
                                  organism, destination));
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (destination.ContainsOrganism())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is occupied by {2}",
                                  organism, destination, destination.Organism));
            }

            source.RemoveOrganism();
            destination.AddOrganism(organism);
            this.OrganismHabitats[organism] = destination;
        }

        public void AddOrganism(Organism organism, Coordinates location)
        {
            var habitat = this.Habitats[location.X, location.Y];
            habitat.AddOrganism(organism);
            this.OrganismHabitats.Add(organism, habitat);
        }

        public void RemoveOrganism(Organism organism)
        {
            var habitat = this.OrganismHabitats[organism];
            habitat.RemoveOrganism();
            this.OrganismHabitats.Remove(organism);
        }

        public void SetTerrain(Coordinates location, Terrain terrain)
        {
            this.Habitats[location.X, location.Y].Environment.SetTerrain(terrain);
        }

        private List<Habitat> GetNeighbouringHabitats(Habitat habitat)
        {
            var neighbouringHabitats = new List<Habitat>();

            var location = this.HabitatCoordinates[habitat];
            for (var x = location.X - 1; x <= location.X + 1; x++)
            {
                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= this.Width)
                {
                    continue;
                }

                for (var y = location.Y - 1; y <= location.Y + 1; y++)
                {
                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= this.Height)
                    {
                        continue;
                    }

                    // do not carry on if (x, y) is diagonal from organism
                    if (x != location.X && y != location.Y)
                    {
                        continue;
                    }

                    neighbouringHabitats.Add(this.Habitats[x, y]);
                }
            }

            return neighbouringHabitats;
        }

        public Dictionary<Measure, double> GetMeasureBiases()
        {
            return new Dictionary<Measure, double> { { Measure.Health, this.HealthBias } };
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismHabitats.Count);
        }
    }
}