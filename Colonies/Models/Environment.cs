﻿namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public sealed class Environment : IEnvironment
    {
        private readonly MeasurementData<EnvironmentMeasure> measurementData;
        public IMeasurementData<EnvironmentMeasure> MeasurementData
        {
            get
            {
                return this.measurementData;
            }
        }

        public bool IsHarmful
        {
            get
            {
                return EnvironmentMeasure.HazardousMeasures()
                    .Any(environmentMeasure => this.measurementData.GetLevel(environmentMeasure).Equals(1.0));
            }
        }

        public Environment()
        {
            var measurements = new List<Measurement<EnvironmentMeasure>>();
            foreach (var measure in Enumeration.GetAll<EnvironmentMeasure>())
            {
                measurements.Add(new Measurement<EnvironmentMeasure>(measure, 0));
            }

            this.measurementData = new MeasurementData<EnvironmentMeasure>(measurements);
        }

        public double GetLevel(EnvironmentMeasure measure)
        {
            return this.measurementData.GetLevel(measure);
        }

        public double SetLevel(EnvironmentMeasure measure, double level)
        {
            return this.measurementData.SetLevel(measure, level);
        }

        public double AdjustLevel(EnvironmentMeasure measure, double adjustment)
        {
            return this.measurementData.AdjustLevel(measure, adjustment);
        }
        
        public override string ToString()
        {
            return this.measurementData.ToString();
        }
    }
}
