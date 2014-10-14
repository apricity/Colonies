namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistoryPull
    {
        IEnumerable<EcosystemModification> Pull();
    }
}
