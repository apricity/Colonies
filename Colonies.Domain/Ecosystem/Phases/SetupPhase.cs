namespace Wacton.Colonies.Domain.Ecosystem.Phases
{
    using System.Linq;

    using Wacton.Colonies.Domain.Ecosystem.Modification;

    public class SetupPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly Afflictor afflictor;

        public SetupPhase(EcosystemData ecosystemData, Afflictor afflictor)
        {
            this.ecosystemData = ecosystemData;
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