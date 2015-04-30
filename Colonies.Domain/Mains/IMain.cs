namespace Wacton.Colonies.Domain.Mains
{
    using Wacton.Colonies.Domain.Ecosystems;
    using Wacton.Colonies.Domain.Ecosystems.Phases;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        PhaseSummary PerformPhase();
    }
}