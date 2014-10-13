namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistoryPullOnly
    {
        IEnumerable<EcosystemModification> Retrieve();
    }
}
