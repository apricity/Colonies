namespace Wacton.Colonies.Domain.Main
{
    using Wacton.Colonies.Domain.Ecosystem;
    using Wacton.Colonies.Domain.Ecosystem.Phases;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        PhaseSummary PerformPhase();
    }
}