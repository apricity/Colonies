namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Models;

    public static class EcosystemLogic
    {
        public static Dictionary<Organism, Habitat> OverrideDesiredOrganismHabitats { get; set; }
        public static Func<IEnumerable<Organism>, Organism> OverrideDecideOrganismFunction { get; set; } 

        public static Dictionary<Organism, Habitat> GetDesiredOrganismHabitats(this Ecosystem ecosystem)
        {
            if (OverrideDesiredOrganismHabitats != null)
            {
                return OverrideDesiredOrganismHabitats;
            }

            var desiredOrganismHabitats = new Dictionary<Organism, Habitat>();
            var aliveOrganismHabitats = ecosystem.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
            foreach (var organismHabitat in aliveOrganismHabitats)
            {
                var currentOrganism = organismHabitat.Key;
                var currentHabitat = organismHabitat.Value;
                var currentCoordinate = ecosystem.HabitatCoordinates[currentHabitat];
                
                // get measurements of neighbouring environments
                var neighbouringHabitats = ecosystem.Habitats.GetNeighbours(currentCoordinate, 1, false, true).ToList();
                var validNeighbouringHabitats = neighbouringHabitats.Where(habitat => habitat != null).ToList();
                var neighbouringEnvironments = validNeighbouringHabitats.Select(neighbour => neighbour.Environment).ToList();

                // determine organism's intentions based on the environment measurements
                var chosenEnvironment = DecisionLogic.MakeDecision(neighbouringEnvironments, currentOrganism);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenHabitat = validNeighbouringHabitats.Single(habitat => habitat.Environment.Equals(chosenEnvironment));
                desiredOrganismHabitats.Add(currentOrganism, chosenHabitat);
            }

            return desiredOrganismHabitats;
        }

        public static Dictionary<Organism, Habitat> ResolveOrganismHabitats(this Ecosystem ecosystem, Dictionary<Organism, Habitat> desiredOrganismHabitats, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismHabitats = new Dictionary<Organism, Habitat>();

            // create a copy of the organism habitats because we don't want to modify the actual set
            var currentOrganismHabitats = ecosystem.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key,
                organismHabitat => organismHabitat.Value);

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                currentOrganismHabitats.Remove(alreadyResolvedOrganism);
            }

            var occupiedHabitats = currentOrganismHabitats.Values.ToList();
            var desiredHabitats = desiredOrganismHabitats.Values.ToList();

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantHabitats = desiredHabitats.Except(occupiedHabitats).Where(habitat => !habitat.IsObstructed()).ToList();
            if (vacantHabitats.Count == 0)
            {
                return resolvedOrganismHabitats;
            }

            foreach (var habitat in vacantHabitats)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantHabitat = habitat;
                var conflictingOrganisms = desiredOrganismHabitats
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantHabitat))
                    .Select(intendedOrganismDestination => intendedOrganismDestination.Key)
                    .ToList();

                Organism organismToMove;
                if (conflictingOrganisms.Count > 1)
                {
                    organismToMove = ecosystem.DecideOrganism(conflictingOrganisms);
                    conflictingOrganisms.Remove(organismToMove);

                    // the remaining conflicting organisms cannot move, so reset their intended destinations
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        desiredOrganismHabitats[remainingOrganism] = ecosystem.OrganismHabitats[remainingOrganism];
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }

                // intended movement becomes an actual, resolved movement
                resolvedOrganismHabitats.Add(organismToMove, desiredOrganismHabitats[organismToMove]);
                desiredOrganismHabitats.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismHabitats.Keys.ToList();
            var trailingOrganismHabitats = ecosystem.ResolveOrganismHabitats(desiredOrganismHabitats, resolvedOrganisms);
            foreach (var trailingOrganismHabitat in trailingOrganismHabitats)
            {
                resolvedOrganismHabitats.Add(trailingOrganismHabitat.Key, trailingOrganismHabitat.Value);
            }

            return resolvedOrganismHabitats;
        }

        private static Organism DecideOrganism(this Ecosystem ecosystem, IEnumerable<Organism> organisms)
        {
            if (OverrideDecideOrganismFunction != null)
            {
                return OverrideDecideOrganismFunction.Invoke(organisms);
            }

            return DecisionLogic.MakeDecision(organisms, ecosystem);
        }

        public static Habitat[,] GetNeighbours(this Habitat[,] habitats, Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            var neighbouringHabitats = new Habitat[(neighbourDepth * 2) + 1, (neighbourDepth * 2) + 1];

            for (var i = -neighbourDepth; i <= neighbourDepth; i++)
            {
                var x = i + coordinate.X;

                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= habitats.Width())
                {
                    continue;
                }

                for (var j = -neighbourDepth; j <= neighbourDepth; j++)
                {
                    var y = j + coordinate.Y;

                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= habitats.Height())
                    {
                        continue;
                    }

                    // do not carry on if (x, y) is diagonal from organism (and include diagonals is false)
                    if (x != coordinate.X && y != coordinate.Y && !includeDiagonals)
                    {
                        continue;
                    }

                    // do not carry on if (x, y) is the centre habitat and asked not to include self
                    if (x == coordinate.X && y == coordinate.Y && !includeSelf)
                    {
                        continue;
                    }

                    neighbouringHabitats[i + neighbourDepth, j + neighbourDepth] = habitats[x, y];
                }
            }

            return neighbouringHabitats;
        }

        public static int CalculateHazardDiameter(this Ecosystem ecosystem)
        {
            var ecosystemArea = (double)(ecosystem.Width * ecosystem.Height);

            var diameterFound = false;
            var currentDiameter = 3; // minimum is 3x3
            while (!diameterFound)
            {
                var nextDiameter = currentDiameter + 2;
                if (Math.Pow(nextDiameter, 2) > Math.Sqrt(ecosystemArea))
                {
                    diameterFound = true;
                }
                else
                {
                    currentDiameter = nextDiameter;
                }
            }

            return currentDiameter;
        }
    }
}
