namespace Wacton.Colonies.Domain.Ecosystems.Modification
{
    using System.Collections.Generic;
    using System.Linq;

    using AForge.Imaging.Filters;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Extensions;
    using Wacton.Colonies.Domain.Measures;

    public class Distributor
    {
        private readonly EcosystemData ecosystemData;
        private readonly Dictionary<EnvironmentMeasure, int> environmentMeasureDiameters;
        private readonly Dictionary<EnvironmentMeasure, List<Coordinate>> hazardSourceCoordinates;

        public Distributor(EcosystemData ecosystemData)
        {
            this.ecosystemData = ecosystemData;

            this.hazardSourceCoordinates = new Dictionary<EnvironmentMeasure, List<Coordinate>>();
            foreach (var environmentMeasure in EnvironmentMeasure.HazardousMeasures())
            {
                this.hazardSourceCoordinates.Add(environmentMeasure, new List<Coordinate>());
            }

            var hazardDiameter = this.ecosystemData.CalculateHazardDiameter();
            this.environmentMeasureDiameters = new Dictionary<EnvironmentMeasure, int>
                {
                    { EnvironmentMeasure.Damp, hazardDiameter },
                    { EnvironmentMeasure.Heat, hazardDiameter },
                    { EnvironmentMeasure.Disease, hazardDiameter },
                    { EnvironmentMeasure.Sound, (hazardDiameter * 4) - 1 }
                };
        }

        public void Insert(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            var diameter = this.environmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;
            var neighbouringCoordinates = this.ecosystemData.GetNeighbours(coordinate, radius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * diameter, diameter).Kernel;
            var gaussianCentre = (double)gaussianKernel[radius, radius];

            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];
                    if (neighbouringCoordinate == null)
                    {
                        continue;
                    }

                    var requiredLevel = gaussianKernel[x, y] / gaussianCentre;
                    var currentLevel = this.ecosystemData.GetLevel(neighbouringCoordinate, environmentMeasure);
                    if (requiredLevel > currentLevel)
                    {
                        this.ecosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, requiredLevel);
                    }
                }
            }

            // TODO: include sound in hazard sources?  or is it better to leave as it is, since organism sound knowledge is held elsewhere?
            if (environmentMeasure.IsHazardous)
            {
                this.RegisterHazardSource(environmentMeasure, coordinate);
            }
        }

        public void Insert(EnvironmentMeasure environmentMeasure, IEnumerable<Coordinate> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                this.Insert(environmentMeasure, coordinate);
            }
        }

        // TODO: review this closely - recently saw a damp of 0.4 with no source next to it...
        public void Remove(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            var diameter = this.environmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;
            var neighbouringCoordinates = this.ecosystemData.GetNeighbours(coordinate, radius, true, true);

            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];
                    if (neighbouringCoordinate == null)
                    {
                        continue;
                    }

                    var requiredLevel = 0;
                    var currentLevel = this.ecosystemData.GetLevel(neighbouringCoordinate, environmentMeasure);
                    if (currentLevel > requiredLevel)
                    {
                        this.ecosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, requiredLevel);
                    }
                }
            }

            if (environmentMeasure.IsHazardous)
            {
                this.DeregisterHazardSource(environmentMeasure, coordinate);
                this.RepairBrokenHazards(environmentMeasure);
            }
        }

        public void Remove(EnvironmentMeasure environmentMeasure, IEnumerable<Coordinate> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                this.Remove(environmentMeasure, coordinate);
            }
        }

        // TODO: not just hazards will be affected - also sound... any distributed measure... will need to deal with this when multiple queens
        private void RepairBrokenHazards(EnvironmentMeasure environmentMeasure)
        {
            // go through all remaining hazard coordinates and restore any remove measures that belonged to other hazards
            var remainingHazardCoordinates = this.hazardSourceCoordinates[environmentMeasure].ToList();
            foreach (var remainingHazardCoordinate in remainingHazardCoordinates)
            {
                var radius = (this.environmentMeasureDiameters[environmentMeasure] - 1) / 2;
                var neighbouringCoordinates = this.ecosystemData.GetNeighbours(remainingHazardCoordinate, radius, true, true).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(coordinate => coordinate != null).ToList();
                if (validNeighbouringCoordinates.All(coordinate => this.ecosystemData.HasLevel(coordinate, environmentMeasure)))
                {
                    continue;
                }

                this.Insert(environmentMeasure, remainingHazardCoordinate);
            }
        }

        public IEnumerable<Coordinate> HazardSources(EnvironmentMeasure environmentMeasure)
        {
            return this.hazardSourceCoordinates[environmentMeasure];
        }

        private void RegisterHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (!this.hazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.hazardSourceCoordinates[environmentMeasure].Add(coordinate);
            }
        }

        private void DeregisterHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (this.hazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.hazardSourceCoordinates[environmentMeasure].Remove(coordinate);
            }
        }
    }
}