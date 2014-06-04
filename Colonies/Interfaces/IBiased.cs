namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    public interface IBiased<T> where T : IMeasure
    {
        Dictionary<T, double> MeasureBiases { get; }
    }
}