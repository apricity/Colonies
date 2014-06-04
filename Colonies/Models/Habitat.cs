namespace Wacton.Colonies.Models
{
    using System;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public sealed class Habitat : IHabitat
    {
        private readonly Environment environment;
        public IEnvironment Environment
        {
            get
            {
                return this.environment;
            }
        }

        private Organism organism;
        public IOrganism Organism
        {
            get
            {
                return this.organism;
            }
        }

        public Habitat(Environment environment, Organism organism)
        {
            this.environment = environment;
            this.organism = organism;
        }

        public void SetOrganism(Organism organismToSet)
        {
            this.organism = organismToSet;
        }

        public void ResetOrganism()
        {
            this.organism = null;
        }

        public bool ContainsOrganism()
        {
            return this.organism != null;
        }

        public bool IsObstructed()
        {
            return this.GetEnvironmentLevel(EnvironmentMeasure.Obstruction) > 0;
        }

        public double GetEnvironmentLevel(EnvironmentMeasure measure)
        {
            return this.Environment.GetLevel(measure);
        }

        public double GetOrganismLevel(OrganismMeasure measure)
        {
            return this.Organism.GetLevel(measure);
        }

        public void SetEnvironmentLevel(EnvironmentMeasure measure, double level)
        {
            this.environment.SetLevel(measure, level);
        }

        public void SetOrganismLevel(OrganismMeasure measure, double level)
        {
            this.organism.SetLevel(measure, level);
        }

        public bool IncreaseEnvironmentLevel(EnvironmentMeasure measure, double increment)
        {
            return this.environment.IncreaseLevel(measure, increment);
        }

        public bool IncreaseOrganismLevel(OrganismMeasure measure, double increment)
        {
            return this.organism.IncreaseLevel(measure, increment);
        }

        public bool DecreaseEnvironmentLevel(EnvironmentMeasure measure, double decrement)
        {
            return this.environment.DecreaseLevel(measure, decrement);
        }

        public bool DecreaseOrganismLevel(OrganismMeasure measure, double decrement)
        {
            return this.organism.DecreaseLevel(measure, decrement);
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.environment, this.organism);
        }

        
    }
}
