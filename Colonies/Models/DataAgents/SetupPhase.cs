namespace Wacton.Colonies.Models.DataAgents
{
    using System.Linq;

    using Wacton.Colonies.Models.Interfaces;

    public class SetupPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly HazardAfflictor hazardAfflictor;

        public SetupPhase(EcosystemData ecosystemData, HazardAfflictor hazardAfflictor)
        {
            this.ecosystemData = ecosystemData;
            this.hazardAfflictor = hazardAfflictor;
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
                this.hazardAfflictor.HazardAffliction(organismCoordinate);
                //this.HazardAfflication(organism, environment);

                var intention = organism.DecideIntention(environment);
                organism.UpdateIntention(intention);
            }
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
    }
}