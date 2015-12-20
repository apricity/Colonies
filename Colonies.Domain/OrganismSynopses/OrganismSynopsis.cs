namespace Wacton.Colonies.Domain.OrganismSynopses
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Organisms;

    public class OrganismSynopsis : IOrganismSynopsis
    {
        public List<IOrganism> Organisms { get; }

        public OrganismSynopsis(List<IOrganism> organisms)
        {
            this.Organisms = organisms;
        }

        public override string ToString() => $"{this.Organisms.Count} organisms";
    }
}
