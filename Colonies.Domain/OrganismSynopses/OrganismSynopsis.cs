namespace Wacton.Colonies.Domain.OrganismSynopses
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Organisms;

    public class OrganismSynopsis : IOrganismSynopsis
    {
        public List<IOrganism> Organisms { get; private set; }

        public OrganismSynopsis(List<IOrganism> organisms)
        {
            this.Organisms = organisms;
        }

        public override string ToString()
        {
            return string.Format("{0} organisms", this.Organisms.Count);
        }
    }
}
