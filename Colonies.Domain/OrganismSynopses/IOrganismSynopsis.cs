namespace Wacton.Colonies.Domain.OrganismSynopses
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Organisms;

    public interface IOrganismSynopsis
    {
        List<IOrganism> Organisms { get; }
    }
}