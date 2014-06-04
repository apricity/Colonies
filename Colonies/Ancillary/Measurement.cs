namespace Wacton.Colonies.Ancillary
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Wacton.Colonies.Interfaces;

    public class Measurement : IMeasurement
    {
        private readonly List<Condition> conditions;
        public IEnumerable<ICondition> Conditions
        {
            get
            {
                return this.conditions;
            }
        }

        public Measurement(List<Condition> conditions)
        {
            this.conditions = conditions;
        }

        public double GetLevel(IMeasure measure)
        {
            return this.GetCondition(measure).Level;
        }

        public void SetLevel(IMeasure measure, double level)
        {
            this.GetCondition(measure).SetLevel(level);
        }

        public bool IncreaseLevel(IMeasure measure, double increment)
        {
            return this.GetCondition(measure).IncreaseLevel(increment);
        }

        public bool DecreaseLevel(IMeasure measure, double decrement)
        {
            return this.GetCondition(measure).DecreaseLevel(decrement);
        }

        public bool HasCondition(Condition condition)
        {
            return this.conditions.Any(condition.Equals);
        }

        private Condition GetCondition(IMeasure measure)
        {
            return this.conditions.Single(condition => condition.Measure.Equals(measure));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var condition in this.conditions)
            {
                stringBuilder.Append(condition);
                stringBuilder.Append("/");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}