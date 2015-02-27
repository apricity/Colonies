namespace Wacton.Colonies.OrganismSynopsis
{
    using System.Collections.Generic;

    using Wacton.Colonies.Organism;

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
