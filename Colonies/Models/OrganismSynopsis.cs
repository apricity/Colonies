namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;

    public class OrganismSynopsis
    {
        public List<Organism> Organisms { get; private set; }

        public OrganismSynopsis(List<Organism> organisms)
        {
            this.Organisms = organisms;
        }

        public override string ToString()
        {
            return string.Format("{0} organisms", this.Organisms.Count);
        }
    }
}
