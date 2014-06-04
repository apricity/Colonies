namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface IEnvironment : IMeasurable<EnvironmentMeasure>
    {
        bool IsHazardous { get; }
    }
}