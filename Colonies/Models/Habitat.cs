namespace Colonies.Models
{
    using System;

    public sealed class Habitat
    {
        public Environment Environment { get; private set; }
        public Organism Organism { get; private set; }

        public Habitat(Environment environment, Organism organism)
        {
            this.Environment = environment;
            this.Organism = organism;
        }

        public void AddOrganism(Organism organism)
        {
            this.Organism = organism;
        }

        public void RemoveOrganism()
        {
            this.Organism = null;
        }

        public bool ContainsOrganism()
        {
            return this.Organism != null;
        }

        public Stimulus GetStimulus()
        {
            // TODO: take into account organisms before return the stimulus
            // e.g. stimulus.Add(organismStimulus)
            var stimulus = this.Environment.GetStimulus();
            return stimulus;
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Environment, this.Organism);
        }
    }
}
