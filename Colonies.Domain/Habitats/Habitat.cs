namespace Wacton.Colonies.Domain.Habitats
{
    using Wacton.Colonies.Domain.Environments;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public sealed class Habitat : IHabitat
    {
        private readonly Environment environment;
        public IEnvironment Environment => this.environment;

        private Organism organism;
        public IOrganism Organism => this.organism;

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

        public override string ToString() => $"{this.environment}, {this.organism}";
    }
}
