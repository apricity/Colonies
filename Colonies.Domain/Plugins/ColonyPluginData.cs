namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Tovarisch.Randomness;
    using Wacton.Tovarisch.Types;

    public class ColonyPluginData
    {
        public Guid ColonyId { get; }
        public string ColonyName { get; }
        public Color ColonyColor { get; private set; }
        public List<WeightedItem<Type>> ColonyLogicTypes { get; }
        public string PluginDescription { get; }

        public ColonyPluginData(IColonyPlugin colonyPlugin)
        {
            this.ColonyId = Guid.NewGuid();
            this.ColonyName = colonyPlugin.ColonyName;
            this.ColonyColor = colonyPlugin.ColonyColor;
            this.ColonyLogicTypes = new List<WeightedItem<Type>>();
            foreach (var logicWeighting in colonyPlugin.LogicWeightings.Get())
            {
                var logicType = logicWeighting.Key;
                var logicWeight = logicWeighting.Value;

                if (!logicType.IsImplementationOf<IOrganismLogic>())
                {
                    continue;
                }

                this.ColonyLogicTypes.Add(new WeightedItem<Type>(logicType, logicWeight));
            }

            this.PluginDescription = colonyPlugin.ToString();
        }

        public override string ToString() => $"Colony: {this.ColonyName} [{this.PluginDescription}] | Logics: {this.ColonyLogicTypes.Count} | ID: {this.ColonyId} ";
    }
}
