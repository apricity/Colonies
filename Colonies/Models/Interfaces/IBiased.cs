namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes.Interfaces;

    public interface IBiased<T> where T : IMeasure
    {
        Dictionary<T, double> MeasureBiases { get; }
    }
}