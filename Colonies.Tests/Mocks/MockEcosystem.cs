namespace Wacton.Colonies.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Models;

    public class MockEcosystem : Ecosystem
    {
        private Dictionary<Organism, Habitat> OverriddenChosenHabitats { get; set; }

        public MockEcosystem(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats, Dictionary<Organism, Habitat> overriddenChosenHabitats)
            : base(habitats, organismHabitats)
        {
            this.OverriddenChosenHabitats = overriddenChosenHabitats;
        }

        protected override Dictionary<Organism, Habitat> GetIntendedOrganismDestinations()
        {
            return this.OverriddenChosenHabitats;
        }

        protected override Organism DecideOrganism(List<Organism> organisms)
        {
            return organisms.First();
        }
    }
}
