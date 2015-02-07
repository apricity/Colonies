namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    public interface IOrganismSynopsis
    {
        List<IOrganism> Organisms { get; }
    }
}