namespace Wacton.Colonies.DataTypes
{
    using System;

    using Wacton.Colonies.DataTypes.Interfaces;

    public class Measurement<T> : IMeasurement<T>, IEquatable<Measurement<T>> where T : IMeasure
    {
        public T Measure { get; private set; }
        public double Level { get; private set; }

        public Measurement(T measure, double level)
        {
            this.Measure = measure;
            this.Level = level;
        }

        public double SetLevel(double level)
        {
            this.Level = Math.Round(level, 4);
            this.EnsureLevelWithinLimit();
            return this.Level;
        }

        public double AdjustLevel(double adjustment)
        {
            // rounding is used to counteract some of the floating point arithmetic loss of precision
            var previousLevel = this.Level;
            this.Level = Math.Round(previousLevel + adjustment, 4);
            this.EnsureLevelWithinLimit();
            return this.Level;
        }

        private void EnsureLevelWithinLimit()
        {
            if (this.Level < 0.0)
            {
                this.Level = 0.0;
            }
            else if (this.Level > 1.0)
            {
                this.Level = 1.0;
            }
        }

        public bool Equals(Measurement<T> other)
        {
            return this.Measure.Equals(other.Measure) && this.Level.Equals(other.Level);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Measure, this.Level);
        }
    }
}