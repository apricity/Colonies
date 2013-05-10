namespace Colonies.Logic
{
    using Colonies.Models;

    using System.Collections.Generic;
    using System.Linq;

    public class ConflictingMovementLogic : IConflictingMovementLogic
    {
        public Organism DecideOrganism(IEnumerable<Organism> organisms)
        {
            // TODO: actual logic to decide on an organism
            return organisms.First();
        }
    }
}
