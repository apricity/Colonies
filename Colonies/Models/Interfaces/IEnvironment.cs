namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes.Enums;

    public interface IEnvironment : IMeasurable<EnvironmentMeasure>
    {
        bool IsHarmful { get; }

        IEnumerable<EnvironmentMeasure> HarmfulMeasures { get; }
    }
}