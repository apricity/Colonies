namespace Wacton.Colonies.Domain.Environments
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Tovarisch.Enum;

    public sealed class Environment : IEnvironment
    {
        private readonly MeasurementData<EnvironmentMeasure> measurementData;
        public IMeasurementData<EnvironmentMeasure> MeasurementData => this.measurementData;

        public bool IsHarmful => this.HarmfulMeasures.Any();
        public IEnumerable<EnvironmentMeasure> HarmfulMeasures
        {
            get
            {
                return EnvironmentMeasure.HazardousMeasures().Where(environmentMeasure => this.GetLevel(environmentMeasure).Equals(1.0));
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

        public override string ToString() => this.measurementData.ToString();
    }
}
