namespace Wacton.Colonies.Domain.Main
{
    using Wacton.Colonies.Domain.Ecosystem;
    using Wacton.Colonies.Domain.Ecosystem.Phases;

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

        private readonly OrganismSynopsis.OrganismSynopsis organismSynopsis;
        public OrganismSynopsis.OrganismSynopsis OrganismSynopsis
        {
            get
            {
                return this.organismSynopsis;
            }
        }

        public Main(Ecosystem ecosystem, OrganismSynopsis.OrganismSynopsis organismSynopsis)
        {
            this.ecosystem = ecosystem;
            this.organismSynopsis = organismSynopsis;
        }

        public override string ToString()
        {
            return this.ecosystem.ToString();
        }

        public PhaseSummary PerformPhase()
        {
            return this.Ecosystem.ExecuteOnePhase();
        }
    }
}
