namespace Wacton.Colonies.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Plugins;

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

        public Dictionary<Type, int> LogicWeightings
        {
            get
            {
                return new Dictionary<Type, int>
                           {
                               { typeof(QueenLogic), 10 },
                               { typeof(GathererLogic), 45 },
                               { typeof(DefenderLogic), 45 }
                           };
            }
        }
    }
}
