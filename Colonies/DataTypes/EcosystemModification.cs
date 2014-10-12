namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;

    public class EcosystemModification
    {
        public List<OrganismModification> OrganismModifications { get; private set; }
        public List<EnvironmentModification> EnvironmentModifications { get; private set; }

        public EcosystemModification()
        {
            this.OrganismModifications = new List<OrganismModification>();
            this.EnvironmentModifications = new List<EnvironmentModification>();
        }

        public void Add(OrganismModification organismModification)
        {
            this.OrganismModifications.Add(organismModification);
        }

        public void Add(IEnumerable<OrganismModification> organismModifications)
        {
            this.OrganismModifications.AddRange(organismModifications);
        }

        public void Add(EnvironmentModification environmentModification)
        {
            this.EnvironmentModifications.Add(environmentModification);
        }

        public void Add(IEnumerable<EnvironmentModification> environmentModifications)
        {
            this.EnvironmentModifications.AddRange(environmentModifications);
        }

        public void Add(EcosystemModification ecosystemModification)
        {
            this.Add(ecosystemModification.OrganismModifications);
            this.Add(ecosystemModification.EnvironmentModifications);
        }

        public override string ToString()
        {
            return string.Format("Organism mods: {0} | Environment mods: {1}", this.OrganismModifications.Count, this.EnvironmentModifications.Count);
        }
    }
}
