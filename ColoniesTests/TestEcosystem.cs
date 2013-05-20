using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoniesTests
{
    using Colonies;
    using Colonies.Models;

    public class TestEcosystem : Ecosystem
    {
        private Dictionary<Organism, Coordinates> OverriddenChosenLocations { get; set; }

        public TestEcosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismLocations, Dictionary<Organism, Coordinates> overridenChosenLocations)
            : base(habitats, organismLocations)
        {
            this.OverriddenChosenLocations = overridenChosenLocations;
        }

        protected override Dictionary<Organism, Coordinates> GetIntendedOrganismDestinations()
        {
            return this.OverriddenChosenLocations;
        }

        protected override Organism ChooseOrganism(List<Organism> conflictingOrganisms)
        {
            return conflictingOrganisms.First();
        }
    }
}
