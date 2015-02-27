namespace Wacton.Colonies.Ecosystem.Phases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Core;
    using Wacton.Colonies.Ecosystem.Modification;
    using Wacton.Colonies.Intentions;
    using Wacton.Colonies.Measures;
    using Wacton.Colonies.Organism;
    using Wacton.Tovarisch.Randomness;

    public class InteractionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly OrganismFactory organismFactory;
        private readonly Afflictor afflictor;
        private readonly Dictionary<Intention, Func<Coordinate, IntentionAdjustments>> interactionFunctions;  

        public InteractionPhase(EcosystemData ecosystemData, OrganismFactory organismFactory, Afflictor afflictor)
        {
            this.ecosystemData = ecosystemData;
            this.organismFactory = organismFactory;
            this.afflictor = afflictor;

            this.interactionFunctions = new Dictionary<Intention, Func<Coordinate, IntentionAdjustments>>
            {
                { Intention.Nourish, this.NourishNeighbour },
                { Intention.Birth, this.BirthOffspring },
            };
        }

        public void Execute()
        {
            this.PerformInteractions();
        }

        private void PerformInteractions()
        {
            foreach (var organismCoordinate in this.ecosystemData.AliveOrganismCoordinates().ToList())
            {
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                if (organism.CanInteract())
                {
                    var adjustments = this.interactionFunctions[organism.CurrentIntention].Invoke(organismCoordinate);
                    this.ecosystemData.AdjustLevels(organismCoordinate, adjustments);
                }
            }
        }

        private IntentionAdjustments BirthOffspring(Coordinate parentOrganismCoordinate)
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

            var offspringOrganismCoordinate = RandomSelection.SelectOne(vacantCoordinates);
            var offspringOrganism = this.organismFactory.CreateOffspringOrganism(parentOrganism);
            this.ecosystemData.AddOrganism(offspringOrganism, offspringOrganismCoordinate);

            this.afflictor.AfflictIfHarmful(offspringOrganismCoordinate);

            var adjustments = parentOrganism.InteractionEffects(offspringOrganism);
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

            var nourishedOrganism = neighboursRequestingNutrient.FirstOrDefault() ?? RandomSelection.SelectOne(neighboursRequestingNutrient);
            var nourishedOrganismCoordinate = this.ecosystemData.CoordinateOf(nourishedOrganism);

            var adjustments = nourishingOrganism.InteractionEffects(nourishedOrganism);
            var givenNutrient = -adjustments.OrganismMeasureAdjustments[OrganismMeasure.Inventory];
            this.ecosystemData.AdjustLevel(nourishedOrganismCoordinate, OrganismMeasure.Health, givenNutrient);

            return adjustments;
        }

        private List<IOrganism> GetNeighboursRequestingNutrient(Coordinate coordinate)
        {
            var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(coordinate, 1, false, false).ToList();
            var neighbourOrganisms = neighbourCoordinates.Select(this.ecosystemData.GetOrganism).Where(organism => organism != null).ToList();
            return neighbourOrganisms.Where(neighbour => neighbour.IsAudible).ToList();
        }
    }
}