namespace Wacton.Colonies.Interfaces
{
    public interface IEnvironment : IMeasurable
    {
        bool IsHazardous { get; }
    }
}