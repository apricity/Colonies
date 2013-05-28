namespace Colonies.Models
{
    using System.Collections.Generic;
    using System.Text;

    public class Measurement
    {
        public List<Condition> Conditions { get; private set; }

        public Measurement(List<Condition> conditions)
        {
            this.Conditions = conditions;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var condition in this.Conditions)
            {
                stringBuilder.Append(condition);
            }

            return stringBuilder.ToString();
        }
    }
}