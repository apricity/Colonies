namespace Colonies.Models
{
    using System.Collections.Generic;

    public class Condition
    {
        public List<Stimulus> Stimuli { get; private set; }

        public Condition(List<Stimulus> stimuli)
        {
            this.Stimuli = stimuli;
        }

        public void SetBias(Factor factor, double bias)
        {
            foreach (var stimulus in this.Stimuli)
            {
                if (stimulus.Factor == factor)
                {
                    stimulus.Bias = bias;
                }
            }
        }
    }
}