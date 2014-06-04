namespace Wacton.Colonies.Interfaces
{
    public interface ICondition
    {
        IMeasure Measure { get; }

        double Level { get; }
    }
}