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
        private readonly OrganismFactory organismFactory;

        public OrganismInteraction(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor, OrganismFactory organismFactory)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
            this.organismFactory = organismFactory;
        }

        public void Execute()
        {
            this.ecosystemData.RefreshOrganismIntentions();
            this.NourishNeighbours();
            this.BirthOrganisms();
        }

        private void BirthOrganisms()
        {
            foreach (var organismCoordinate in this.ecosystemData.OrganismCoordinates(Intention.Birth).ToList())
            {
                var parentOrganism = this.ecosystemData.GetOrganism(organismCoordinate);
                if (parentOrganism.GetLevel(OrganismMeasure.Inventory) < 1.0)
                {
                    continue;
                }

                var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(organismCoordinate, 1, false, false).ToList();
                var vacantCoordinates = neighbourCoordinates.Where(coordinate =>
                        !this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction)
                        && this.ecosystemData.GetOrganism(coordinate) == null).ToList();

                if (!vacantCoordinates.Any())
                {
                    continue;
                }

                var childCoordinate = DecisionLogic.MakeDecision(vacantCoordinates);
                var childOrganism = this.organismFactory.CreateChildOrganism(parentOrganism);
                this.ecosystemData.AddOrganism(childOrganism, childCoordinate);
                this.ecosystemData.AdjustLevel(organismCoordinate, OrganismMeasure.Inventory, -1.0);
            }
        }

        private void NourishNeighbours()
        {
            foreach (var organismCoordinate in this.ecosystemData.OrganismCoordinates(Intention.Nourish))
            {
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                if (!organism.CanInteractOrganism())
                {
                    continue;
                }

                var neighboursRequestingNutrient = this.GetNeighboursRequestingNutrient(organismCoordinate);
                if (neighboursRequestingNutrient.Any())
                {
                    var nourishedOrganism = neighboursRequestingNutrient.FirstOrDefault() ?? DecisionLogic.MakeDecision(neighboursRequestingNutrient);
                    var nourishedOrganismCoordinate = this.ecosystemData.CoordinateOf(nourishedOrganism);

                    var adjustments = organism.InteractOrganismAdjustments(nourishedOrganism);
                    this.ecosystemData.AdjustLevels(organismCoordinate, adjustments);

                    var givenNutrient = -adjustments.OrganismMeasureAdjustments[OrganismMeasure.Inventory];
                    this.ecosystemData.AdjustLevel(nourishedOrganismCoordinate, OrganismMeasure.Health, givenNutrient);
                }
            }
        }

        private List<IOrganism> GetNeighboursRequestingNutrient(Coordinate coordinate)
        {
            // TODO: better way?!  perhaps organism tries to give food to anyone audible (even if not queen)
            var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(coordinate, 1, false, false).ToList();
            var neighbourOrganisms = neighbourCoordinates.Select(this.ecosystemData.GetOrganism).Where(organism => organism != null).ToList();
            return neighbourOrganisms.Where(neighbour => neighbour.CurrentIntention.Equals(Intention.Reproduce) && !neighbour.IsReproductive).ToList();
        }
    }
}