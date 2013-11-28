namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface ICondition
    {
        Measure Measure { get; }

        double Level { get; }
    }
}