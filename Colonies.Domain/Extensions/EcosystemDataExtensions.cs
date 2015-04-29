namespace Wacton.Colonies.Domain.Extensions
{
    using System;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystem.Modification;

    public static class EcosystemDataExtensions
    {
        public static Coordinate[,] GetNeighbours(this EcosystemData ecosystemData, Coordinate coordinate, int neighbourDepth, bool includeDiagonals, bool includeSelf)
        {
            var neighbouringCoordinates = new Coordinate[(neighbourDepth * 2) + 1, (neighbourDepth * 2) + 1];

            for (var i = -neighbourDepth; i <= neighbourDepth; i++)
            {
                var x = i + coordinate.X;

                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= ecosystemData.Width)
                {
                    continue;
                }

                for (var j = -neighbourDepth; j <= neighbourDepth; j++)
                {
                    var y = j + coordinate.Y;

                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= ecosystemData.Height)
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

        public static int CalculateHazardDiameter(this EcosystemData ecosystemData)
        {
            var ecosystemArea = (double)(ecosystemData.Width * ecosystemData.Height);

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
