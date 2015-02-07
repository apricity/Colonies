namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;

    public interface IEcosystemHistoryPusher
    {
        void Push(EcosystemModification modification);

        void Push(EcosystemRelocation relocation);

        void Push(EcosystemAddition addition);
    }
}
