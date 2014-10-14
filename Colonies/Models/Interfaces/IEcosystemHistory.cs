namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistory
    {
        List<EcosystemModification> Modifications { get; }

        List<EcosystemRelocation> Relocations { get; }
    }
}
