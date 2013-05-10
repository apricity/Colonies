namespace Colonies.Logic
{
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IConflictingMovementLogic
    {
        Organism DecideOrganism(IEnumerable<Organism> organisms);
    }
}