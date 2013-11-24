namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Models;

    public static class EcosystemLogic
    {
        public static Dictionary<Organism, Coordinate> OverrideDesiredOrganismCoordinates { get; set; }
        public static Func<IEnumerable<Organism>, Organism> OverrideDecideOrganismFunction { get; set; } 

        public static Dictionary<Organism, Coordinate> GetDesiredOrganismHabitats(EcosystemData ecosystemData)
        {
            if (OverrideDesiredOrganismCoordinates != null)
            {
                return OverrideDesiredOrganismCoordinates;
            }

            var desiredOrganismCoordinates = new Dictionary<Organism, Coordinate>();
            var aliveOrganisms = ecosystemData.GetOrganisms().Where(organism => organism.IsAlive).ToList();
            foreach (var organism in aliveOrganisms)
            {
                var currentCoordinate = ecosystemData.CoordinateOf(organism);
                
                // get measurements of neighbouring environments
                var neighbourCoordinates = ecosystemData.GetNeighbours(currentCoordinate, 1, false, true).ToList();
                var validNeighbourCoordinates = neighbourCoordinates.Where(habitat => habitat != null).ToList();
                var neighbourMeasurables = validNeighbourCoordinates.Select(ecosystemData.GetMeasurable).ToList();

                // determine organism's intentions based on the environment measurements
                var chosenMeasurable = DecisionLogic.MakeDecision(neighbourMeasurables, organism);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenCoordinate = validNeighbourCoordinates.Single(coordinate => ecosystemData.GetMeasurable(coordinate).Equals(chosenMeasurable));
                desiredOrganismCoordinates.Add(organism, chosenCoordinate);
            }

            return desiredOrganismCoordinates;
        }

        public static Dictionary<Organism, Coordinate> ResolveOrganismHabitats(EcosystemData ecosystemData, Dictionary<Organism, Coordinate> desiredOrganismCoordinates, IEnumerable<Organism> alreadyResolvedOrganisms, IBiased biasProvider)
        {
            var resolvedOrganismCoordinates = new Dictionary<Organism, Coordinate>();

            // create a copy of the organism habitats because we don't want to modify the actual set
            var organisms = ecosystemData.GetOrganisms().ToList();

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                organisms.Remove(alreadyResolvedOrganism);
            }

            var occupiedCoordinates = organisms.Select(ecosystemData.CoordinateOf).ToList();
            var desiredCoordinates = desiredOrganismCoordinates.Values;

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantCoordinates = desiredCoordinates
                .Except(occupiedCoordinates)
                .Where(coordinate => !ecosystemData.HasMeasure(coordinate, Measure.Obstruction))
                .Distinct().ToList();
            if (vacantCoordinates.Count == 0)
            {
                return resolvedOrganismCoordinates;
            }

            foreach (var coordinate in vacantCoordinates)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantCoordinate = coordinate;
                var conflictingOrganisms = desiredOrganismCoordinates
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantCoordinate))
                    .Select(intendedOrganismDestination => intendedOrganismDestination.Key)
                    .ToList();

                Organism organismToMove;
                if (conflictingOrganisms.Count > 1)
                {
                    organismToMove = DecideOrganism(biasProvider, conflictingOrganisms);
                    conflictingOrganisms.Remove(organismToMove);

                    // the remaining conflicting organisms cannot move, so reset their intended destinations
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        desiredOrganismCoordinates[remainingOrganism] = ecosystemData.CoordinateOf(remainingOrganism);
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }

                // intended movement becomes an actual, resolved movement
                resolvedOrganismCoordinates.Add(organismToMove, desiredOrganismCoordinates[organismToMove]);
                desiredOrganismCoordinates.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismCoordinates.Keys.ToList();
            var trailingOrganismHabitats = ResolveOrganismHabitats(ecosystemData, desiredOrganismCoordinates, resolvedOrganisms, biasProvider);
            foreach (var trailingOrganismHabitat in trailingOrganismHabitats)
            {
                resolvedOrganismCoordinates.Add(trailingOrganismHabitat.Key, trailingOrganismHabitat.Value);
            }

            return resolvedOrganismCoordinates;
        }

        private static Organism DecideOrganism(IBiased biasProvider, IEnumerable<Organism> organisms)
        {
            if (OverrideDecideOrganismFunction != null)
            {
                return OverrideDecideOrganismFunction.Invoke(organisms);
            }

            return DecisionLogic.MakeDecision(organisms, biasProvider);
        }

        public static Coordinate[,] GetNeighbours(this Habitat[,] habitats, Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            var neighbouringCoordinates = new Coordinate[(neighbourDepth * 2) + 1, (neighbourDepth * 2) + 1];

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

                    neighbouringCoordinates[i + neighbourDepth, j + neighbourDepth] = new Coordinate(x, y);
                }
            }

            return neighbouringCoordinates;
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
