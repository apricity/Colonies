namespace Wacton.Colonies.Domain.Ecosystems.Data
{
    public interface IEcosystemHistoryPusher
    {
        void Push(EcosystemModification modification);

        void Push(EcosystemRelocation relocation);

        void Push(EcosystemAddition addition);
    }
}
