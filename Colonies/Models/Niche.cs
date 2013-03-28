using System;

namespace Colonies
{
    public sealed class Niche
    {
        public Habitat Habitat { get; set; }
        public Organism Organism { get; set; }

        public Niche(Habitat habitat, Organism organism)
        {
            this.Habitat = habitat;
            this.Organism = organism;
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Habitat, this.Organism);
        }
    }
}
