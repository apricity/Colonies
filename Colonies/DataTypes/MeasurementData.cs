namespace Wacton.Colonies.DataTypes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Wacton.Colonies.DataTypes.Interfaces;

    public class MeasurementData<T> : IMeasurementData<T> where T : IMeasure
    {
        private readonly List<Measurement<T>> measurements;
        public IEnumerable<IMeasurement<T>> Measurements
        {
            get
            {
                return this.measurements;
            }
        }

        public MeasurementData(List<Measurement<T>> measurements)
        {
            this.measurements = measurements;
        }

        public double GetLevel(IMeasure measure)
        {
            return this.GetMeasurement(measure).Level;
        }

        public double SetLevel(IMeasure measure, double level)
        {
            return this.GetMeasurement(measure).SetLevel(level);
        }

        public double AdjustLevel(IMeasure measure, double adjustment)
        {
            return this.GetMeasurement(measure).AdjustLevel(adjustment);
        }

        private Measurement<T> GetMeasurement(IMeasure measure)
        {
            return this.measurements.Single(measurement => measurement.Measure.Equals(measure));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var measurement in this.measurements)
            {
                stringBuilder.Append(measurement);
                stringBuilder.Append("/");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}