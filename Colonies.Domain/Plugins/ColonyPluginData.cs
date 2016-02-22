namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Tovarisch.Collections;
    using Wacton.Tovarisch.Randomness;

    public class ColonyPluginData
    {
        public Guid ColonyId { get; }
        public string ColonyName { get; }
        public Color ColonyColor { get; private set; }
        public List<WeightedItem<IOrganismLogic>> ColonyLogics { get; }
        public string PluginDescription { get; }

        public ColonyPluginData(IColonyPlugin colonyPlugin)
        {
            CheckForDuplicates(colonyPlugin.ColonyLogics.Keys);

            this.ColonyId = Guid.NewGuid();
            this.ColonyName = colonyPlugin.ColonyName;
            this.ColonyColor = colonyPlugin.ColonyColor;
            this.ColonyLogics = new List<WeightedItem<IOrganismLogic>>();
            foreach (var logicWeighting in colonyPlugin.ColonyLogics)
            {
                var organismLogic = logicWeighting.Key;
                var logicWeight = logicWeighting.Value;

                this.ColonyLogics.Add(new WeightedItem<IOrganismLogic>(organismLogic, logicWeight));
            }

            this.PluginDescription = colonyPlugin.ToString();
        }

        private static void CheckForDuplicates(IEnumerable<IOrganismLogic> colonyLogics)
        {
            var duplicates = colonyLogics.Select(item => item.GetType().Name).Duplicates();
            if (!duplicates.Any())
            {
                return;
            }

            var duplicatesString = duplicates.ToDelimitedString(", ");
            throw new InvalidOperationException($"Colony plugin has duplicated logics ({duplicatesString})");
        }

        public override string ToString() => $"Colony: {this.ColonyName} [{this.PluginDescription}] | Logics: {this.ColonyLogics.Count} | ID: {this.ColonyId} ";
    }
}
