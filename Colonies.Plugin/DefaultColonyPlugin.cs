namespace Wacton.Colonies.Plugin
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Plugins;

    [Export(typeof(IColonyPlugin))]
    public class DefaultColonyPlugin : IColonyPlugin
    {
        public string ColonyName => "Default";
        public Color ColonyColor => Colors.Silver;

        public Dictionary<IOrganismLogic, int> ColonyLogics =>
                new Dictionary<IOrganismLogic, int>
                {
                    { new QueenLogic(), 10 },
                    { new GathererLogic(), 45 },
                    { new DefenderLogic(), 45 }
                };
    }
}
