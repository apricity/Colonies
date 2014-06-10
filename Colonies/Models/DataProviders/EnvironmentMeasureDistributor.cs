namespace Wacton.Colonies.Models.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;

    using AForge.Imaging.Filters;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Logic;

    public class EnvironmentMeasureDistributor
    {
        private EcosystemData EcosystemData { get; set; }
        private Dictionary<EnvironmentMeasure, int> EnvironmentMeasureDiameters { get; set; } 

        public EnvironmentMeasureDistributor(EcosystemData ecosystemData)
        {
            this.EcosystemData = ecosystemData;

            var hazardDiameter = this.EcosystemData.CalculateHazardDiameter();
            this.EnvironmentMeasureDiameters = new Dictionary<EnvironmentMeasure, int>
                {
                    { EnvironmentMeasure.Damp, hazardDiameter },
                    { EnvironmentMeasure.Heat, hazardDiameter },
                    { EnvironmentMeasure.Poison, hazardDiameter },
                    { EnvironmentMeasure.Sound, (hazardDiameter * 4) - 1 }
                };
        }

        public IEnumerable<Coordinate> InsertDistribution(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var diameter = this.EnvironmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;

            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, radius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * diameter, diameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[radius, radius];
            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];

                    if (neighbouringCoordinate != null
                        && level > this.EcosystemData.GetLevel(neighbouringCoordinate, environmentMeasure))
                    {
                        this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, level);
                        alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                    }
                }
            }

            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.InsertHazard(environmentMeasure, coordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> RemoveDistribution(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

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

                    this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, 0);
                    alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                }
            }

            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.RemoveHazard(environmentMeasure, coordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> RandomAddHazards(EnvironmentMeasure environmentMeasure, double addChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            if (!DecisionLogic.IsSuccessful(addChance))
            {
                return alteredEnvironmentCoordinates;
            }

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            var nonHazardCoordinates = this.EcosystemData.AllCoordinates().Except(hazardCoordinates);
            var chosenNonHazardCoordinate = DecisionLogic.MakeDecision(nonHazardCoordinates);
            if (!this.EcosystemData.HasLevel(chosenNonHazardCoordinate, EnvironmentMeasure.Obstruction))
            {
                alteredEnvironmentCoordinates.AddRange(this.InsertDistribution(chosenNonHazardCoordinate, environmentMeasure));
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> RandomSpreadHazards(EnvironmentMeasure environmentMeasure, double spreadChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(spreadChance))
                {
                    continue;
                }

                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(hazardCoordinate, 1, false, false).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbourCoordinate =>
                    neighbourCoordinate != null
                    && !this.EcosystemData.HasLevel(neighbourCoordinate, EnvironmentMeasure.Obstruction)
                    && this.EcosystemData.GetLevel(neighbourCoordinate, environmentMeasure) < 1).ToList();

                if (validNeighbouringCoordinates.Count == 0)
                {
                    continue;
                }

                var chosenCoordinate = DecisionLogic.MakeDecision(validNeighbouringCoordinates);
                alteredEnvironmentCoordinates.AddRange(this.InsertDistribution(chosenCoordinate, environmentMeasure));
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> RandomRemoveHazards(EnvironmentMeasure environmentMeasure, double removeChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(removeChance))
                {
                    continue;
                }

                alteredEnvironmentCoordinates.AddRange(this.RemoveDistribution(hazardCoordinate, environmentMeasure));
            }

            // go through all remaining hazard coordinates and restore any remove measures that belonged to other hazards
            var remainingHazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var remainingHazardCoordinate in remainingHazardCoordinates)
            {
                var radius = (this.EnvironmentMeasureDiameters[environmentMeasure] - 1) / 2;
                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(remainingHazardCoordinate, radius, true, true).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(coordinate => coordinate != null).ToList();
                if (validNeighbouringCoordinates.Any(coordinate =>!this.EcosystemData.HasLevel(coordinate, environmentMeasure)))
                {
                    alteredEnvironmentCoordinates.AddRange(this.InsertDistribution(remainingHazardCoordinate, environmentMeasure));
                }
            }

            return alteredEnvironmentCoordinates;
        }
    }
}
