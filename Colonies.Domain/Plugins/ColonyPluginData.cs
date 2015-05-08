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
        public Guid ColonyId { get; private set; }
        public string ColonyName { get; private set; }
        public Color ColonyColor { get; private set; }
        public List<WeightedItem<Type>> ColonyLogicTypes { get; private set; }

        public ColonyPluginData(IColonyPlugin colonyPlugin)
        {
            this.ColonyId = Guid.NewGuid();
            this.ColonyName = colonyPlugin.ColonyName;
            this.ColonyColor = colonyPlugin.ColonyColor;
            this.ColonyLogicTypes = new List<WeightedItem<Type>>();
            foreach (var logicWeighting in colonyPlugin.LogicWeightings)
            {
                var logicType = logicWeighting.Key;
                var logicWeight = logicWeighting.Value;

                if (!logicType.IsImplementationOf<IOrganismLogic>())
                {
                    continue;
                }

                this.ColonyLogicTypes.Add(new WeightedItem<Type>(logicType, logicWeight));
            }
        }

        public override string ToString()
        {
            return string.Format("Colony: {0} | Logics: {1} | ID: {2}", this.ColonyName, this.ColonyLogicTypes.Count, this.ColonyId);
        }
    }
}
