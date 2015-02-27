namespace Wacton.Colonies.Environment
{
    using System.Collections.Generic;

    using Wacton.Colonies.Measures;

    public interface IEnvironment : IMeasurable<EnvironmentMeasure>
    {
        bool IsHarmful { get; }

        IEnumerable<EnvironmentMeasure> HarmfulMeasures { get; }
    }
}