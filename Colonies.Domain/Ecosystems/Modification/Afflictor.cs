namespace Wacton.Colonies.Domain.Ecosystems.Modification
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Measures;

    public class Afflictor
    {
        private readonly EcosystemData ecosystemData;
        private readonly Distributor distributor;
        private readonly Dictionary<EnvironmentMeasure, Action<Coordinate>> hazardActions;

        public Afflictor(EcosystemData ecosystemData, Distributor distributor)
        {
            this.ecosystemData = ecosystemData;
            this.distributor = distributor;

            this.hazardActions
                = new Dictionary<EnvironmentMeasure, Action<Coordinate>>
                {
                    { EnvironmentMeasure.Heat, this.AfflictHeat },
                    { EnvironmentMeasure.Damp, this.AfflictDamp },
                    { EnvironmentMeasure.Disease, this.AfflictDisease }
                };
        }

        public void AfflictIfHarmful(Coordinate organismCoordinate)
        {
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
            if (!environment.IsHarmful)
            {
                return;
            }

            foreach (var harmfulMeasure in environment.HarmfulMeasures)
            {
                this.hazardActions[harmfulMeasure].Invoke(organismCoordinate);
            }
        }

        private void AfflictHeat(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            organism.OverloadPheromone();
        }

        private void AfflictDamp(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            organism.OverloadSound();

            // insert sound distribution immediately, regardless of the phase this is called in
            this.distributor.Insert(EnvironmentMeasure.Sound, organismCoordinate);
        }

        private void AfflictDisease(Coordinate organismCoordinate)
        {
            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            organism.ContractDisease();

            // not required - the only way to contract disease is by being on a coordinate that already has disease distribution
            //this.distributor.Insert(organismCoordinate, EnvironmentMeasure.Disease);
        }
    }
}
