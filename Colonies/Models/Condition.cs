namespace Wacton.Colonies.Models
{
    using System;

    public class Condition
    {
        public Measure Measure { get; private set; }
        public double Level { get; private set; }

        public Condition(Measure measure, double level)
        {
            this.Measure = measure;
            this.Level = level;
        }

        public void SetLevel(double level)
        {
            this.Level = level;
            this.EnsureLevelWithinLimit();
        }

        public bool IncreaseLevel(double increment)
        {
            return this.ChangeLevel(increment);
        }

        public bool DecreaseLevel(double decrement)
        {
            return this.ChangeLevel(-decrement);
        }

        private bool ChangeLevel(double value)
        {
            var previousLevel = this.Level;
            this.Level += value;
            this.EnsureLevelWithinLimit();
            return Math.Abs(previousLevel - this.Level) > 0.0;
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

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Measure, this.Level);
        }
    }
}