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
        private readonly Dictionary<Intention, Func<Coordinate, IntentionAdjustments>> organismInteractionFunctions;  

        public OrganismInteraction(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor, OrganismFactory organismFactory)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
            this.organismFactory = organismFactory;

            this.organismInteractionFunctions = new Dictionary<Intention, Func<Coordinate, IntentionAdjustments>>
            {
                { Intention.Nourish, organismCoordinate => this.NourishNeighbour(organismCoordinate) },
                { Intention.Birth, organismCoordinate => this.BirthOrganism(organismCoordinate) },
            };
        }

        public void Execute()
        {
            this.ecosystemData.RefreshOrganismIntentions();
            this.PerformInteractions();
        }

        private void PerformInteractions()
        {
            foreach (var organismCoordinate in this.ecosystemData.AliveOrganismCoordinates().ToList())
            {
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                if (organism.CanInteractOrganism())
                {
                    var adjustments = this.organismInteractionFunctions[organism.CurrentIntention].Invoke(organismCoordinate);
                    this.ecosystemData.AdjustLevels(organismCoordinate, adjustments);
                }
            }
        }

        private IntentionAdjustments BirthOrganism(Coordinate parentOrganismCoordinate)
        {
            var parentOrganism = this.ecosystemData.GetOrganism(parentOrganismCoordinate);

            var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(parentOrganismCoordinate, 1, false, false).ToList();
            var vacantCoordinates = neighbourCoordinates.Where(coordinate =>
                    !this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction)
                    && this.ecosystemData.GetOrganism(coordinate) == null).ToList();

            if (!vacantCoordinates.Any())
            {
                return new IntentionAdjustments();
            }

            var childOrganismCoordinate = DecisionLogic.MakeDecision(vacantCoordinates);
            var childOrganism = this.organismFactory.CreateChildOrganism(parentOrganism);
            this.ecosystemData.AddOrganism(childOrganism, childOrganismCoordinate);

            var adjustments = parentOrganism.InteractOrganismAdjustments(childOrganism);
            return adjustments;
        }

        private IntentionAdjustments NourishNeighbour(Coordinate nourishingOrganismCoordinate)
        {
            var nourishingOrganism = this.ecosystemData.GetOrganism(nourishingOrganismCoordinate);

            var neighboursRequestingNutrient = this.GetNeighboursRequestingNutrient(nourishingOrganismCoordinate);
            if (!neighboursRequestingNutrient.Any())
            {
                return new IntentionAdjustments();
            }

            var nourishedOrganism = neighboursRequestingNutrient.FirstOrDefault() ?? DecisionLogic.MakeDecision(neighboursRequestingNutrient);
            var nourishedOrganismCoordinate = this.ecosystemData.CoordinateOf(nourishedOrganism);

            var adjustments = nourishingOrganism.InteractOrganismAdjustments(nourishedOrganism);
            var givenNutrient = -adjustments.OrganismMeasureAdjustments[OrganismMeasure.Inventory];
            this.ecosystemData.AdjustLevel(nourishedOrganismCoordinate, OrganismMeasure.Health, givenNutrient);

            return adjustments;
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