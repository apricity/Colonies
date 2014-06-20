﻿namespace Wacton.Colonies.DataTypes.Interfaces
{
    using System.Collections.Generic;

    public interface IMeasurementData<T> where T : IMeasure
    {
        IEnumerable<IMeasurement<T>> Measurements { get; }

        double GetLevel(IMeasure measure);
    }
}
