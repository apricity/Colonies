namespace Wacton.Colonies.Models
{
    using System;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

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
            return this.GetLevel(EnvironmentMeasure.Obstruction) > 0;
        }

        public double GetLevel(EnvironmentMeasure measure)
        {
            return this.Environment.GetLevel(measure);
        }

        public double GetLevel(OrganismMeasure measure)
        {
            return this.Organism.GetLevel(measure);
        }

        public double SetLevel(EnvironmentMeasure measure, double level)
        {
            return this.environment.SetLevel(measure, level);
        }

        public double SetLevel(OrganismMeasure measure, double level)
        {
            return this.organism.SetLevel(measure, level);
        }

        public double AdjustLevel(EnvironmentMeasure measure, double adjustment)
        {
            return this.environment.AdjustLevel(measure, adjustment);
        }

        public double AdjustLevel(OrganismMeasure measure, double adjustment)
        {
            return this.organism.AdjustLevel(measure, adjustment);
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.environment, this.organism);
        }
    }
}
