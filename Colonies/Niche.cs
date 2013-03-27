using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    public class Niche
    {
        public Habitat Habitat { get; set; }
        public Organism Organism { get; set; }

        public Niche(Habitat habitat, Organism organism)
        {
            this.Habitat = habitat;
            this.Organism = organism;
        }
    }
}
