namespace Colonies.Models
{
    using System.Collections.Generic;

    public class Measurement
    {
        public List<Condition> Conditions { get; private set; }
        public double Weighting { get; private set; }

        public Measurement(List<Condition> conditions)
        {
            this.Conditions = conditions;
        }

        public void SetWeighting(double weighting)
        {
            this.Weighting = weighting;
        }
    }
}