namespace Wacton.Colonies.Main
{
    using Wacton.Colonies.Ecosystem;
    using Wacton.Colonies.Ecosystem.Phases;

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

        public Main(Ecosystem ecosystem)
        {
            this.ecosystem = ecosystem;
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
