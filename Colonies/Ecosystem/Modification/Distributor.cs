namespace Wacton.Colonies.Ecosystem.Modification
{
    using System.Collections.Generic;
    using System.Linq;

    using AForge.Imaging.Filters;

    using Wacton.Colonies.Core;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Measures;

    public class Distributor
    {
        private EcosystemData EcosystemData { get; set; }
        private Dictionary<EnvironmentMeasure, int> EnvironmentMeasureDiameters { get; set; }
        private Dictionary<EnvironmentMeasure, List<Coordinate>> HazardSourceCoordinates { get; set; }

        public Distributor(EcosystemData ecosystemData)
        {
            this.EcosystemData = ecosystemData;

            this.HazardSourceCoordinates = new Dictionary<EnvironmentMeasure, List<Coordinate>>();
            foreach (var environmentMeasure in EnvironmentMeasure.HazardousMeasures())
            {
                this.HazardSourceCoordinates.Add(environmentMeasure, new List<Coordinate>());
            }

            var hazardDiameter = this.EcosystemData.CalculateHazardDiameter();
            this.EnvironmentMeasureDiameters = new Dictionary<EnvironmentMeasure, int>
                {
                    { EnvironmentMeasure.Damp, hazardDiameter },
                    { EnvironmentMeasure.Heat, hazardDiameter },
                    { EnvironmentMeasure.Disease, hazardDiameter },
                    { EnvironmentMeasure.Sound, (hazardDiameter * 4) - 1 }
                };
        }

        public void Insert(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            var diameter = this.EnvironmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;
            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, radius, true, true);
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
                    var currentLevel = this.EcosystemData.GetLevel(neighbouringCoordinate, environmentMeasure);
                    if (requiredLevel > currentLevel)
                    {
                        this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, requiredLevel);
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
            var diameter = this.EnvironmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;
            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, radius, true, true);

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
                    var currentLevel = this.EcosystemData.GetLevel(neighbouringCoordinate, environmentMeasure);
                    if (currentLevel > requiredLevel)
                    {
                        this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, requiredLevel);
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
            var remainingHazardCoordinates = this.HazardSourceCoordinates[environmentMeasure].ToList();
            foreach (var remainingHazardCoordinate in remainingHazardCoordinates)
            {
                var radius = (this.EnvironmentMeasureDiameters[environmentMeasure] - 1) / 2;
                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(remainingHazardCoordinate, radius, true, true).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(coordinate => coordinate != null).ToList();
                if (validNeighbouringCoordinates.All(coordinate => this.EcosystemData.HasLevel(coordinate, environmentMeasure)))
                {
                    continue;
                }

                this.Insert(environmentMeasure, remainingHazardCoordinate);
            }
        }

        public IEnumerable<Coordinate> HazardSources(EnvironmentMeasure environmentMeasure)
        {
            return this.HazardSourceCoordinates[environmentMeasure];
        }

        private void RegisterHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (!this.HazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardSourceCoordinates[environmentMeasure].Add(coordinate);
            }
        }

        private void DeregisterHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate)
        {
            if (this.HazardSourceCoordinates[environmentMeasure].Contains(coordinate))
            {
                this.HazardSourceCoordinates[environmentMeasure].Remove(coordinate);
            }
        }
    }
}