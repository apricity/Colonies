namespace Wacton.Colonies.Domain.OrganismSynopsis
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Organism;

    public interface IOrganismSynopsis
    {
        List<IOrganism> Organisms { get; }
    }
}