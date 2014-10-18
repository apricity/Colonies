namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class OrganismInteraction : IEcosystemStage
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;

        public OrganismInteraction(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
        }

        public void Execute()
        {
            this.ecosystemData.RefreshOrganismIntentions();
            this.NourishNeighbours();
        }

        private void NourishNeighbours()
        {
            foreach (var organismCoordinate in this.ecosystemData.OrganismCoordinates(Intention.Nourish))
            {
                var neighboursRequestingNutrient = this.GetNeighboursRequestingNutrient(organismCoordinate);
                if (neighboursRequestingNutrient.Any())
                {
                    var nourishedOrganism = neighboursRequestingNutrient.FirstOrDefault() ?? DecisionLogic.MakeDecision(neighboursRequestingNutrient);
                    var nourishedOrganismCoordinate = this.ecosystemData.CoordinateOf(nourishedOrganism);

                    var desiredNutrient = 1 - this.ecosystemData.GetLevel(nourishedOrganismCoordinate, OrganismMeasure.Health);
                    var availableNutrient = this.ecosystemData.GetLevel(organismCoordinate, OrganismMeasure.Inventory);
                    var givenNutrient = Math.Min(desiredNutrient, availableNutrient);
                    this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, -givenNutrient);
                    this.ecosystemData.AdjustLevel(nourishedOrganismCoordinate, OrganismMeasure.Health, givenNutrient);

                    if (!nourishedOrganism.NeedsAssistance)
                    {
                        this.environmentMeasureDistributor.RemoveDistribution(nourishedOrganismCoordinate, EnvironmentMeasure.Sound);
                    }
                }
            }
        }

        private List<IOrganism> GetNeighboursRequestingNutrient(Coordinate coordinate)
        {
            // TODO: better way?!  perhaps organism tries to give food to anyone who needs assistance (even if not queen)
            var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(coordinate, 1, false, false).ToList();
            var neighbourOrganisms = neighbourCoordinates.Select(this.ecosystemData.GetOrganism).Where(organism => organism != null).ToList();
            return neighbourOrganisms.Where(neighbour => neighbour.Intention.Equals(Intention.Reproduce) && neighbour.NeedsAssistance).ToList();
        }
    }
}