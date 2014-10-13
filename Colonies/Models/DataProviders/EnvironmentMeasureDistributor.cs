﻿namespace Wacton.Colonies.Models.DataProviders
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

        public void InsertDistribution(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
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

            // TODO: not good!  is there a good way round using CheckHazardStatus?
            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.InsertHazardSource(environmentMeasure, coordinate);
            }
        }

        public void RemoveDistribution(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
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

            // TODO: not good!  is there a good way round using CheckHazardStatus?
            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.RemoveHazardSource(environmentMeasure, coordinate);
                this.RepairBrokenHazards(environmentMeasure);
            }
        }

        public void RandomAddHazards(EnvironmentMeasure environmentMeasure, double addChance)
        {
            if (!DecisionLogic.IsSuccessful(addChance))
            {
                return;
            }

            var hazardCoordinates = this.EcosystemData.GetHazardSourceCoordinates(environmentMeasure).ToList();
            var nonHazardCoordinates = this.EcosystemData.AllCoordinates().Except(hazardCoordinates);
            var chosenNonHazardCoordinate = DecisionLogic.MakeDecision(nonHazardCoordinates);
            if (!this.EcosystemData.HasLevel(chosenNonHazardCoordinate, EnvironmentMeasure.Obstruction))
            {
                this.InsertDistribution(chosenNonHazardCoordinate, environmentMeasure);
            }
        }

        public void RandomSpreadHazards(EnvironmentMeasure environmentMeasure, double spreadChance)
        {
            var hazardCoordinates = this.EcosystemData.GetHazardSourceCoordinates(environmentMeasure).ToList();
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
                this.InsertDistribution(chosenCoordinate, environmentMeasure);
            }
        }

        public void RandomRemoveHazards(EnvironmentMeasure environmentMeasure, double removeChance)
        {
            var hazardCoordinates = this.EcosystemData.GetHazardSourceCoordinates(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(removeChance))
                {
                    continue;
                }

                this.RemoveDistribution(hazardCoordinate, environmentMeasure);
            }
        }

        // TODO: not just hazards will be affected - also sound... any distributed measure... will need to deal with this when multiple queens
        private void RepairBrokenHazards(EnvironmentMeasure environmentMeasure)
        {
            // go through all remaining hazard coordinates and restore any remove measures that belonged to other hazards
            var remainingHazardCoordinates = this.EcosystemData.GetHazardSourceCoordinates(environmentMeasure).ToList();
            foreach (var remainingHazardCoordinate in remainingHazardCoordinates)
            {
                var radius = (this.EnvironmentMeasureDiameters[environmentMeasure] - 1) / 2;
                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(remainingHazardCoordinate, radius, true, true).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(coordinate => coordinate != null).ToList();
                if (validNeighbouringCoordinates.All(coordinate => this.EcosystemData.HasLevel(coordinate, environmentMeasure)))
                {
                    continue;
                }

                this.InsertDistribution(remainingHazardCoordinate, environmentMeasure);
            }
        }
    }
}
