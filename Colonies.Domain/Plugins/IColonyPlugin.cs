namespace Wacton.Colonies.Domain.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    public interface IColonyPlugin
    {
        string ColonyName { get; }

        // TODO: consider moving to RGB values if wanting to avoid plugins using transparency
        Color ColonyColor { get; }

        PluginLogicWeightings LogicWeightings { get; }
    }
}
