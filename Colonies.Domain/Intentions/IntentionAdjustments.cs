namespace Wacton.Colonies.Domain.Intentions
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;

    public class IntentionAdjustments
    {
        public readonly Dictionary<OrganismMeasure, double> OrganismMeasureAdjustments;
        public readonly Dictionary<EnvironmentMeasure, double> EnvironmentMeasureAdjustments; 

        public IntentionAdjustments()
        {
            this.OrganismMeasureAdjustments = new Dictionary<OrganismMeasure, double>();
            this.EnvironmentMeasureAdjustments = new Dictionary<EnvironmentMeasure, double>();
        }

        public IntentionAdjustments(Dictionary<OrganismMeasure, double> organismMeasureAdjustments, Dictionary<EnvironmentMeasure, double> environmentMeasureAdjustments)
        {
            this.OrganismMeasureAdjustments = organismMeasureAdjustments;
            this.EnvironmentMeasureAdjustments = environmentMeasureAdjustments;
        }
    }
}
