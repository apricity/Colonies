namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Models;

    public static class EcosystemLogic
    {
        //public static Dictionary<Organism, Habitat> GetDesiredOrganismHabitats(this Ecosystem ecosystem)
        //{
        //    var desiredOrganismHabitats = new Dictionary<Organism, Habitat>();
        //    var aliveOrganismHabitats = ecosystem.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
        //    foreach (var organismHabitat in aliveOrganismHabitats)
        //    {
        //        var currentOrganism = organismHabitat.Key;
        //        var currentHabitat = organismHabitat.Value;
        //        var currentCoordinate = ecosystem.HabitatCoordinates[currentHabitat];
                
        //        // get measurements of neighbouring environments
        //        var neighbouringHabitats = ecosystem.Habitats.GetNeighbours(currentCoordinate, 1, false, true).ToList();
        //        var validNeighbouringHabitats = neighbouringHabitats.Where(habitat => habitat != null).ToList();
        //        var neighbouringEnvironments = validNeighbouringHabitats.Select(neighbour => neighbour.Environment).ToList();

        //        // determine organism's intentions based on the environment measurements
        //        var chosenEnvironment = DecisionLogic.MakeDecision(neighbouringEnvironments, currentOrganism);

        //        // get the habitat the environment is from - this is where the organism wants to move to
        //        var chosenHabitat = validNeighbouringHabitats.Single(habitat => habitat.Environment.Equals(chosenEnvironment));
        //        desiredOrganismHabitats.Add(currentOrganism, chosenHabitat);
        //    }

        //    return desiredOrganismHabitats;
        //}

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

        public static int CalculateHazardDiameter(int ecosystemWidth, int ecosystemHeight)
        {
            var ecosystemArea = (double)(ecosystemWidth * ecosystemHeight);

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
