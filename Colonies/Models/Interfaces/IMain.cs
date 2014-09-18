namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes;

    public interface IMain
    {
        IEcosystem Ecosystem { get; }

        UpdateSummary PerformUpdate();
    }
}