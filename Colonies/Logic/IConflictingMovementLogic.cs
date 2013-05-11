namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IConflictingMovementLogic
    {
        Organism DecideOrganism(IEnumerable<Organism> organisms, double healthWeighting, Random random);
    }
}