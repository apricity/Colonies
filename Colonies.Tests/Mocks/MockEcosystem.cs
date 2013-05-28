namespace Colonies.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Linq;

    using Colonies;
    using Colonies.Models;

    public class MockEcosystem : Ecosystem
    {
        private Dictionary<Organism, Coordinates> OverriddenChosenLocations { get; set; }

        public MockEcosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismLocations, Dictionary<Organism, Coordinates> overridenChosenLocations)
            : base(habitats, organismLocations)
        {
            this.OverriddenChosenLocations = overridenChosenLocations;
        }

        protected override Dictionary<Organism, Coordinates> GetIntendedOrganismDestinations()
        {
            return this.OverriddenChosenLocations;
        }

        protected override Organism DecideOrganism(List<Organism> organisms)
        {
            return organisms.First();
        }
    }
}
