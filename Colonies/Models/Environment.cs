namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.DataTypes.Interfaces;
    using Wacton.Colonies.Models.Interfaces;

    public sealed class Environment : IEnvironment
    {
        private readonly MeasurementData measurementData;
        public IMeasurementData MeasurementData
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
            var measurements = new List<Measurement>();
            foreach (var measure in Enumeration.GetAll<EnvironmentMeasure>())
            {
                measurements.Add(new Measurement(measure, 0));
            }

            this.measurementData = new MeasurementData(measurements);
        }

        public double GetLevel(EnvironmentMeasure testMeasure)
        {
            return this.measurementData.GetLevel(testMeasure);
        }

        public void SetLevel(EnvironmentMeasure measure, double level)
        {
            this.measurementData.SetLevel(measure, level);
        }

        public bool IncreaseLevel(EnvironmentMeasure measure, double increment)
        {
            return this.measurementData.IncreaseLevel(measure, increment);
        }

        public bool DecreaseLevel(EnvironmentMeasure measure, double decrement)
        {
            return this.measurementData.DecreaseLevel(measure, decrement);
        }
        
        public override string ToString()
        {
            return this.measurementData.ToString();
        }
    }
}
