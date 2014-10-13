namespace Wacton.Colonies.Models.DataProviders
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;

    public class EcosystemHistory
    {
        private readonly List<Modification> modifications;

        public EcosystemHistory()
        {
            this.modifications = new List<Modification>();
        }

        public void Record(Modification modification)
        {
            this.modifications.Add(modification);
        }

        public IEnumerable<Modification> Retrieve()
        {
            var duplicate = new Modification[this.modifications.Count];
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
