namespace Wacton.Colonies.Domain.Mains
{
    using Wacton.Colonies.Domain.Ecosystems;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.OrganismSynopses;

    public class Main : IMain
    {
        private readonly Ecosystem ecosystem;
        public IEcosystem Ecosystem => this.ecosystem;

        private readonly OrganismSynopsis organismSynopsis;
        public OrganismSynopsis OrganismSynopsis => this.organismSynopsis;

        public Main(Ecosystem ecosystem, OrganismSynopsis organismSynopsis)
        {
            this.ecosystem = ecosystem;
            this.organismSynopsis = organismSynopsis;
        }

        public PhaseSummary PerformPhase()
        {
            return this.Ecosystem.ExecuteOnePhase();
        }

        public override string ToString() => this.ecosystem.ToString();
    }
}
