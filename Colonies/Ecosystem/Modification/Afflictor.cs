namespace Wacton.Colonies.Ecosystem.Modification
{
    using System;
    using System.Collections.Generic;

    using Wacton.Colonies.Core;
    using Wacton.Colonies.Measures;

    public class Afflictor
    {
        private EcosystemData EcosystemData { get; set; }
        private Distributor Distributor { get; set; }
        private readonly Dictionary<EnvironmentMeasure, Action<Coordinate>> hazardActions;

        public Afflictor(EcosystemData ecosystemData, Distributor distributor)
        {
            this.EcosystemData = ecosystemData;
            this.Distributor = distributor;

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
            var environment = this.EcosystemData.GetEnvironment(organismCoordinate);
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
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.OverloadPheromone();
        }

        private void AfflictDamp(Coordinate organismCoordinate)
        {
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.OverloadSound();

            // insert sound distribution immediately, regardless of the phase this is called in
            this.Distributor.Insert(EnvironmentMeasure.Sound, organismCoordinate);
        }

        private void AfflictDisease(Coordinate organismCoordinate)
        {
            var organism = this.EcosystemData.GetOrganism(organismCoordinate);
            organism.ContractDisease();

            // not required - the only way to contract disease is by being on a coordinate that already has disease distribution
            //this.Distributor.Insert(organismCoordinate, EnvironmentMeasure.Disease);
        }
    }
}
