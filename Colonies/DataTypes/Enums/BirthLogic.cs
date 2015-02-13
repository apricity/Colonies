﻿namespace Wacton.Colonies.DataTypes.Enums
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Models.Interfaces;

    public class BirthLogic : IIntentionLogic
    {
        public Inventory AssociatedIntenvory
        {
            get
            {
                return Inventory.Spawn;
            }
        }

        public Dictionary<EnvironmentMeasure, double> EnvironmentBias
        {
            get
            {
                return new Dictionary<EnvironmentMeasure, double>
                       {
                           { EnvironmentMeasure.Damp, -10 },
                           { EnvironmentMeasure.Heat, -10 },
                           { EnvironmentMeasure.Disease, -50 }
                       };
            }
        }

        public bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return false;
        }

        public IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment, IOrganismState organismState)
        {
            return new IntentionAdjustments();
        }

        public bool CanInteractOrganism(IOrganismState organismState)
        {
            return this.OrganismHasSpawn(organismState);
        }

        public IntentionAdjustments InteractOrganismAdjustments(IOrganismState organismState, IOrganismState otherOrganismState)
        {
            if (!this.CanInteractOrganism(organismState))
            {
                return new IntentionAdjustments();
            }

            var organismAdjustments = new Dictionary<OrganismMeasure, double>();
            var environmentAdjustments = new Dictionary<EnvironmentMeasure, double>();
            organismAdjustments.Add(OrganismMeasure.Inventory, -1.0);

            return new IntentionAdjustments(organismAdjustments, environmentAdjustments);
        }

        private bool OrganismHasSpawn(IOrganismState organismState)
        {
            return organismState.CurrentInventory.Equals(Inventory.Spawn) && organismState.GetLevel(OrganismMeasure.Inventory).Equals(1.0);
        }
    }
}
