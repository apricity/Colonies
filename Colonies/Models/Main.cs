namespace Wacton.Colonies.Models
{
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

        public Main(Ecosystem ecosystem)
        {
            this.ecosystem = ecosystem;
        }

        public override string ToString()
        {
            return this.ecosystem.ToString();
        }

        public UpdateSummary PerformUpdate()
        {
            return this.Ecosystem.UpdateOneStage();
        }
    }
}
