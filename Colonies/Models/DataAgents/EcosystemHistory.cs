namespace Wacton.Colonies.Models.DataAgents
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemHistory : IEcosystemHistory, IEcosystemHistoryPuller, IEcosystemHistoryPusher
    {
        public List<EcosystemModification> Modifications { get; private set; }
        public List<EcosystemRelocation> Relocations { get; private set; }

        public EcosystemHistory()
        {
            this.Modifications = new List<EcosystemModification>();
            this.Relocations = new List<EcosystemRelocation>();
        }

        public EcosystemHistory(List<EcosystemModification> modifications, List<EcosystemRelocation> relocations)
        {
            this.Modifications = modifications;
            this.Relocations = relocations;
        }

        public void Push(EcosystemModification modification)
        {
            this.Modifications.Add(modification);
        }

        public void Push(EcosystemRelocation relocation)
        {
            this.Relocations.Add(relocation);
        }

        public IEcosystemHistory Pull()
        {
            var modifications = new EcosystemModification[this.Modifications.Count];
            this.Modifications.CopyTo(modifications);
            this.Modifications.Clear();

            var relocations = new EcosystemRelocation[this.Relocations.Count];
            this.Relocations.CopyTo(relocations);
            this.Relocations.Clear();

            return new EcosystemHistory(modifications.ToList(), relocations.ToList());
        }

        public override string ToString()
        {
            return string.Format("Modifications: {0}", this.Modifications.Count);
        }
    }
}
