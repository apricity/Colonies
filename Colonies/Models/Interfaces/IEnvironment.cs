namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes.Enums;

    public interface IEnvironment : IMeasurable<EnvironmentMeasure>
    {
        bool IsHarmful { get; }
    }
}