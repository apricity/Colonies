namespace Wacton.Colonies.Main
{
    using Wacton.Colonies.Ecosystem;
    using Wacton.Colonies.Ecosystem.Phases;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        PhaseSummary PerformPhase();
    }
}