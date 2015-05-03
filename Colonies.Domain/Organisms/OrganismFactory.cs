namespace Wacton.Colonies.Domain.Organisms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Tovarisch.Lexicon;
    using Wacton.Tovarisch.Numbers;
    using Wacton.Tovarisch.Randomness;
    using Wacton.Tovarisch.Strings;

    public class OrganismFactory
    {
        // TODO: this will have to be modifiable when custom organism plugins come about
        private readonly List<WeightedItem<Type>> organismTypeWeightings = new List<WeightedItem<Type>>
                {
                    new WeightedItem<Type>(typeof(Queen), 10),
                    new WeightedItem<Type>(typeof(Gatherer), 45),
                    new WeightedItem<Type>(typeof(Defender), 45)
                };

        private readonly WordProvider wordProvider = new WordProvider();
        private readonly List<string> usedNames = new List<string>();

        public IOrganism CreateOffspringOrganism(IOrganism parentOrganism)
        {
            var firstName = this.wordProvider.GetRandomWord(WordClass.Verb).ToTitleCase();
            var secondName = this.wordProvider.GetRandomWord(WordClass.Noun).ToTitleCase();
            var name = string.Concat(firstName, " ", secondName);

            var nameCount = this.usedNames.Count(usedName => usedName.Equals(name));
            this.usedNames.Add(name);

            // update name with numeral suffix (II, III, IV, etc.) if name already exists
            if (nameCount > 0)
            {
                var numeralSuffix = nameCount + 1;
                name = string.Concat(name, " ", numeralSuffix.ToRomanNumerals());
            }

            var color = parentOrganism.Color;
            var organismType = RandomSelection.SelectOne(this.organismTypeWeightings);
            var organism = (IOrganism)Activator.CreateInstance(organismType, name, color);
            return organism;
        }
    }
}
