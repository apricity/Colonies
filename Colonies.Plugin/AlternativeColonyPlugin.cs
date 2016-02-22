namespace Wacton.Colonies.Plugin
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Plugins;

    [Export(typeof(IColonyPlugin))]
    public class AlternativeColonyPlugin : IColonyPlugin
    {
        public string ColonyName => "Alternative";
        public Color ColonyColor => Colors.Fuchsia;

        public Dictionary<IOrganismLogic, int> ColonyLogics =>
                new Dictionary<IOrganismLogic, int>
                {
                    { new KamikazeLogic(), 1 }
                };
    }
}
