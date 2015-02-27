namespace Wacton.Colonies.Ecosystem.Data
{
    public interface IEcosystemHistoryPusher
    {
        void Push(EcosystemModification modification);

        void Push(EcosystemRelocation relocation);

        void Push(EcosystemAddition addition);
    }
}
