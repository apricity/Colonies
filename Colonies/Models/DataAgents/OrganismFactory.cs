namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class OrganismFactory
    {
        // chance of queen is 10% - all others have equal chance
        public IOrganism CreateChildOrganism(IOrganism parentOrganism)
        {
            // TODO: this is just a quick way of getting a random string - use a dictionary of adjectives & nouns (or similar) instead
            var name = Path.GetRandomFileName().Replace(".", string.Empty);
            var color = parentOrganism.Color;

            var isChildQueen = DecisionLogic.IsSuccessful(0.05);
            if (isChildQueen)
            {
                return new Queen(name, color);
            }

            var organismConstructionFunctions = new List<Func<Organism>>
                                                    {
                                                        () => new Defender(name, color),
                                                        () => new Gatherer(name, color)
                                                    };

            var organismConstructionFunction = DecisionLogic.MakeDecision(organismConstructionFunctions);
            var organism = organismConstructionFunction.Invoke();
            return organism;
        }
    }
}
