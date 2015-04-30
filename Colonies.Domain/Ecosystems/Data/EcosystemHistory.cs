namespace Wacton.Colonies.Domain.Ecosystems.Data
{
    using System.Collections.Generic;
    using System.Linq;

    public class EcosystemHistory : IEcosystemHistory, IEcosystemHistoryPuller, IEcosystemHistoryPusher
    {
        public List<EcosystemModification> Modifications { get; private set; }
        public List<EcosystemRelocation> Relocations { get; private set; }
        public List<EcosystemAddition> Additions { get; private set; } 

        public EcosystemHistory()
        {
            this.Modifications = new List<EcosystemModification>();
            this.Relocations = new List<EcosystemRelocation>();
            this.Additions = new List<EcosystemAddition>();
        }

        public EcosystemHistory(List<EcosystemModification> modifications, List<EcosystemRelocation> relocations, List<EcosystemAddition> additions)
        {
            this.Modifications = modifications;
            this.Relocations = relocations;
            this.Additions = additions;
        }

        public void Push(EcosystemModification modification)
        {
            this.Modifications.Add(modification);
        }

        public void Push(EcosystemRelocation relocation)
        {
            this.Relocations.Add(relocation);
        }

        public void Push(EcosystemAddition addition)
        {
            this.Additions.Add(addition);
        }

        public IEcosystemHistory Pull()
        {
            var modifications = new EcosystemModification[this.Modifications.Count];
            this.Modifications.CopyTo(modifications);
            this.Modifications.Clear();

            var relocations = new EcosystemRelocation[this.Relocations.Count];
            this.Relocations.CopyTo(relocations);
            this.Relocations.Clear();

            var additions = new EcosystemAddition[this.Additions.Count];
            this.Additions.CopyTo(additions);
            this.Additions.Clear();

            return new EcosystemHistory(modifications.ToList(), relocations.ToList(), additions.ToList());
        }

        public override string ToString()
        {
            return string.Format("Modifications: {0}", this.Modifications.Count);
        }
    }
}
