namespace Wacton.Colonies.Models.DataAgents
{
    using System.Linq;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class SetupPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly Distributor distributor;
        private readonly Afflictor afflictor;

        public SetupPhase(EcosystemData ecosystemData, Distributor distributor, Afflictor afflictor)
        {
            this.ecosystemData = ecosystemData;
            this.distributor = distributor;
            this.afflictor = afflictor;
        }

        public void Execute()
        {
            this.PerformSetup();
        }

        private void PerformSetup()
        {
            foreach (var organismCoordinate in this.ecosystemData.AliveOrganismCoordinates().ToList())
            {
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
                this.afflictor.AfflictIfHarmful(organismCoordinate);

                var intention = organism.DecideIntention(environment);
                organism.UpdateIntention(intention);
            }
        }
    }
}