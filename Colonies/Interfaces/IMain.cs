namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        UpdateSummary UpdateOnce();
    }
}