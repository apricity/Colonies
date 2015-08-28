namespace Wacton.Colonies.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Wacton.Colonies.Domain.Plugins;

    [Export(typeof(IColonyPlugin))]
    public class DefaultColonyPlugin : IColonyPlugin
    {
        public string ColonyName
        {
            get
            {
                return "Default";
            }
        }

        public Color ColonyColor
        {
            get
            {
                return Colors.Silver;
            }
        }

        public PluginLogicWeightings LogicWeightings
        {
            get
            {
                var logicWeightings = new PluginLogicWeightings();
                logicWeightings.Add<QueenLogic>(10);
                logicWeightings.Add<GathererLogic>(45);
                logicWeightings.Add<DefenderLogic>(45);
                return logicWeightings;
            }
        }
    }
}
