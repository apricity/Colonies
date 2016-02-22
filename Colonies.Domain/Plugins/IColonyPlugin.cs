namespace Wacton.Colonies.Domain.Plugins
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Organisms;

    public interface IColonyPlugin
    {
        string ColonyName { get; }

        // TODO: consider moving to RGB values if wanting to avoid plugins using transparency
        Color ColonyColor { get; }

        Dictionary<IOrganismLogic, int> ColonyLogics { get; }
    }
}
