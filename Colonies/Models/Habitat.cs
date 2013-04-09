namespace Colonies.Models
{
    using System;

    public sealed class Habitat
    {
        public Environment Environment { get; set; }
        public Organism Organism { get; set; }

        public Habitat(Environment environment, Organism organism)
        {
            this.Environment = environment;
            this.Organism = organism;
        }

        public bool ContainsOrganism()
        {
            return this.Organism != null;
        }

        public Features GetFeatures()
        {
            return new Features(1);
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Environment, this.Organism);
        }
    }
}
