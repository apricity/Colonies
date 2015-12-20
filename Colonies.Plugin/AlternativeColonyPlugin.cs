namespace Wacton.Colonies.Plugin
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Plugins;

    [Export(typeof(IColonyPlugin))]
    public class AlternativeColonyPlugin : IColonyPlugin
    {
        public string ColonyName => "Alternative";
        public Color ColonyColor => Colors.Fuchsia;

        public PluginLogicWeightings LogicWeightings
        {
            get
            {
                var logicWeightings = new PluginLogicWeightings();
                logicWeightings.Add<Kamikaze>(1);
                return logicWeightings;
            }
        }
    }
}
