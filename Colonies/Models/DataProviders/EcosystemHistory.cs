namespace Wacton.Colonies.Models.DataProviders
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemHistory : IEcosystemHistoryPullOnly, IEcosystemHistoryPushOnly
    {
        private readonly List<EcosystemModification> modifications;

        public EcosystemHistory()
        {
            this.modifications = new List<EcosystemModification>();
        }

        public void Record(EcosystemModification modification)
        {
            this.modifications.Add(modification);
        }

        public IEnumerable<EcosystemModification> Retrieve()
        {
            var duplicate = new EcosystemModification[this.modifications.Count];
            this.modifications.CopyTo(duplicate);
            this.modifications.Clear();
            return duplicate;
        }

        public override string ToString()
        {
            return string.Format("Modifications: {0}", this.modifications.Count);
        }
    }
}
