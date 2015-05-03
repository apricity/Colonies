namespace Wacton.Colonies.Domain.Organisms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

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
        private readonly Dictionary<Guid, string> colonyNames = new Dictionary<Guid, string>();

        public IOrganism CreateOrphanOrganism(Color color, Type organismType)
        {
            var colonyId = Guid.NewGuid();
            var colonyName = this.GenerateColonyName(colonyId);
            var name = this.GenerateFullName(colonyName);
            var organism = (IOrganism)Activator.CreateInstance(organismType, colonyId, name, color);
            EnsureOrganismInstantiatedCorrectly(organism, colonyId, name, color);
            return organism;
        }

        public IOrganism CreateOffspringOrganism(IOrganism parentOrganism)
        {
            var colonyId = parentOrganism.ColonyId;
            var colonyName = this.colonyNames[colonyId];
            var name = this.GenerateFullName(colonyName);
            var color = parentOrganism.Color;
            var organismType = RandomSelection.SelectOne(this.organismTypeWeightings);
            var organism = (IOrganism)Activator.CreateInstance(organismType, parentOrganism.ColonyId, name, color);
            EnsureOrganismInstantiatedCorrectly(organism, colonyId, name, color);
            return organism;
        }

        private string GenerateColonyName(Guid colonyId)
        {
            var colonyName = string.Empty;

            var isUniqueColonyName = false;
            while (!isUniqueColonyName)
            {
                colonyName = this.wordProvider.GetRandomWord(WordClass.Adjective).ToTitleCase();
                if (!this.colonyNames.ContainsValue(colonyName))
                {
                    isUniqueColonyName = true;
                }
            }

            this.colonyNames.Add(colonyId, colonyName);
            return colonyName;
        }

        private string GenerateFullName(string colonyName)
        {
            var givenName = this.wordProvider.GetRandomWord(WordClass.Noun).ToTitleCase();
            var nickName = this.wordProvider.GetRandomWord(WordClass.Verb).ToTitleCase();
            var name = string.Format("{0} \"{1}\" {2}", givenName, nickName, colonyName);

            var nameCount = this.usedNames.Count(usedName => usedName.Equals(name));
            this.usedNames.Add(name);

            // update name with numeral suffix (II, III, IV, etc.) if name already exists
            if (nameCount > 0)
            {
                var numeralSuffix = nameCount + 1;
                name = string.Concat(name, " ", numeralSuffix.ToRomanNumerals());
            }

            return name;
        }

        private static void EnsureOrganismInstantiatedCorrectly(IOrganism organism, Guid colonyId, string name, Color color)
        {
            if (organism.ColonyId != colonyId)
            {
                throw new InvalidOperationException(
                    string.Format("Organism instance was not created as requested | expected ID {0}, actual ID {1}",
                    colonyId, organism.ColonyId));
            }

            if (organism.Name != name)
            {
                throw new InvalidOperationException(
                    string.Format("Organism instance was not created as requested | expected name {0}, actual name {1}",
                    name, organism.Name));
            }

            if (organism.Color != color)
            {
                throw new InvalidOperationException(
                    string.Format("Organism instance was not created as requested | expected color {0}, actual color {1}",
                    color, organism.Color));
            }
        }
    }
}
