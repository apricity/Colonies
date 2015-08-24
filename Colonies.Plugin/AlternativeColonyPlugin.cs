namespace Wacton.Colonies.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Plugins;

    [Export(typeof(IColonyPlugin))]
    public class AlternativeColonyPlugin : IColonyPlugin
    {
        public string ColonyName
        {
            get
            {
                return "Alternative";
            }
        }

        public Color ColonyColor
        {
            get
            {
                return Colors.Fuchsia;
            }
        }

        public Dictionary<Type, int> LogicWeightings
        {
            get
            {
                return new Dictionary<Type, int>
                           {
                               { typeof(Kamikaze), 1 }
                           };
            }
        }
    }
}
