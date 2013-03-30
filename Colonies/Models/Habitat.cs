namespace Colonies.Models
{
    using System;
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.ViewModels;

    using Microsoft.Practices.Prism.Events;

    public sealed class Habitat
    {
        private Environment environment;
        public Environment Environment
        {
            get
            {
                return this.environment;
            }
            set
            {
                this.environment = value;
            }
        }

        private Organism organism;
        public Organism Organism
        {
            get
            {
                return this.organism;
            }
            set
            {
                this.organism = value;
            }
        }

        public Habitat(Environment environment, Organism organism)
        {
            this.Environment = environment;
            this.Organism = organism;
        }

        public bool ContainsOrganism()
        {
            return this.Organism != null;
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Environment, this.Organism);
        }
    }
}
