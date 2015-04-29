namespace Wacton.Colonies.Domain.Ecosystem.Data
{
    using System.Collections.Generic;

    public interface IEcosystemHistory
    {
        List<EcosystemModification> Modifications { get; }

        List<EcosystemRelocation> Relocations { get; }

        List<EcosystemAddition> Additions { get; }
    }
}
