namespace Wacton.Colonies.OrganismSynopsis
{
    using System.Collections.Generic;

    using Wacton.Colonies.Organism;

    public interface IOrganismSynopsis
    {
        List<IOrganism> Organisms { get; }
    }
}