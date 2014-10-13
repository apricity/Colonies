namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistoryPushOnly
    {
        void Record(EcosystemModification modification);
    }
}
