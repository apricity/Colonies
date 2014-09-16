namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        IEnumerable<UpdateSummary> PerformUpdates();

        UpdateSummary UpdateOnce();
    }
}