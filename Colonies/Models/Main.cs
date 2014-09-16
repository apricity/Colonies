namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.Models.Interfaces;

    public class Main : IMain
    {
        private readonly Ecosystem ecosystem;
        public IEcosystem Ecosystem
        {
            get
            {
                return this.ecosystem;
            }
        }

        private readonly List<Func<IEnumerable<Coordinate>>> updateStages;

        public Main(Ecosystem ecosystem)
        {
            this.ecosystem = ecosystem;
            this.updateStages = new List<Func<IEnumerable<Coordinate>>>
                                    {
                                        this.Ecosystem.PerformEnvironmentInteractions,
                                        this.Ecosystem.PerformMovementsActions,
                                        this.Ecosystem.PerformOrganismInteractions,
                                        this.Ecosystem.PerformEcosystemModifiers
                                    };
        }

        public override string ToString()
        {
            return this.ecosystem.ToString();
        }

        public IEnumerable<UpdateSummary> PerformUpdates()
        {
            foreach (var updateStage in this.updateStages)
            {
                var previousOrganismCoordinates = this.Ecosystem.OrganismCoordinates();
                var alteredEnvironmentCoordinates = updateStage.Invoke().ToList();
                var currentOrganismCoordinates = this.Ecosystem.OrganismCoordinates();
                var updateSummary = new UpdateSummary(previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates);
                yield return updateSummary;
            }
        }

        public UpdateSummary UpdateOnce()
        {
            return this.ecosystem.Update();
        }
    }
}
