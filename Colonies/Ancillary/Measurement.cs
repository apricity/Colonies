namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Measurement
    {
        public List<Condition> Conditions { get; private set; }

        public Measurement(List<Condition> conditions)
        {
            this.Conditions = conditions;
        }

        public double GetLevel(Measure measure)
        {
            return this.GetCondition(measure).Level;
        }

        public void SetLevel(Measure measure, double level)
        {
            this.GetCondition(measure).SetLevel(level);
        }

        public bool IncreaseLevel(Measure measure, double increment)
        {
            return this.GetCondition(measure).IncreaseLevel(increment);
        }

        public bool DecreaseLevel(Measure measure, double decrement)
        {
            return this.GetCondition(measure).DecreaseLevel(decrement);
        }

        public bool HasCondition(Condition condition)
        {
            return this.Conditions.Any(condition.Equals);
        }

        private Condition GetCondition(Measure measure)
        {
            return this.Conditions.Single(condition => condition.Measure.Equals(measure));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var condition in this.Conditions)
            {
                stringBuilder.Append(condition);
                stringBuilder.Append("/");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}