namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistoryPush
    {
        void Push(EcosystemModification modification);
    }
}
