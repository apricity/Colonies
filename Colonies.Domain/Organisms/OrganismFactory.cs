namespace Wacton.Colonies.Domain.Organisms
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Wacton.Tovarisch.Randomness;

    public class OrganismFactory
    {
        // TODO: this will have to be modifiable when custom organism plugins come about
        private readonly List<WeightedItem<Type>> organismTypeWeightings = new List<WeightedItem<Type>>
                {
                    new WeightedItem<Type>(typeof(Queen), 10),
                    new WeightedItem<Type>(typeof(Gatherer), 45),
                    new WeightedItem<Type>(typeof(Defender), 45)
                };

        public IOrganism CreateOffspringOrganism(IOrganism parentOrganism)
        {
            // TODO: this is just a quick way of getting a random string - use a dictionary of adjectives & nouns (or similar) instead
            var name = Path.GetRandomFileName().Replace(".", string.Empty);
            var color = parentOrganism.Color;

            var organismType = RandomSelection.SelectOne(this.organismTypeWeightings);
            var organism = (IOrganism)Activator.CreateInstance(organismType, name, color);
            return organism;
        }
    }
}
