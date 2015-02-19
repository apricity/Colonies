﻿namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class InteractionPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;
        private readonly OrganismFactory organismFactory;
        private readonly HazardAfflictor hazardAfflictor;
        private readonly Dictionary<Intention, Func<Coordinate, IntentionAdjustments>> interactionFunctions;  

        public InteractionPhase(EcosystemData ecosystemData, EnvironmentMeasureDistributor environmentMeasureDistributor, OrganismFactory organismFactory, HazardAfflictor hazardAfflictor)
        {
            this.ecosystemData = ecosystemData;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
            this.organismFactory = organismFactory;
            this.hazardAfflictor = hazardAfflictor;

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

            var offspringOrganismCoordinate = DecisionLogic.MakeDecision(vacantCoordinates);
            var offspringOrganism = this.organismFactory.CreateOffspringOrganism(parentOrganism);
            var offspringEnvironment = this.ecosystemData.GetEnvironment(offspringOrganismCoordinate);
            this.ecosystemData.AddOrganism(offspringOrganism, offspringOrganismCoordinate);

            // TODO: test!
            this.hazardAfflictor.HazardAffliction(offspringOrganismCoordinate);
            //this.HazardAfflication(offspringOrganism, offspringEnvironment);

            var adjustments = parentOrganism.InteractionEffects(offspringOrganism);
            return adjustments;
        }

        //private void HazardAfflication(IOrganism organism, IEnvironment environment)
        //{
        //    if (!environment.IsHarmful)
        //    {
        //        return;
        //    }

        //    foreach (var harmfulMeasure in environment.HarmfulMeasures)
        //    {
        //        harmfulMeasure.OrganismAfflication.Invoke(organism);
        //    }
        //}

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