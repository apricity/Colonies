namespace Wacton.Colonies.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Models;

    public class MockEcosystem : Ecosystem
    {
        private Dictionary<Organism, Habitat> OverriddenDesiredHabitats { get; set; }

        public MockEcosystem(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats, Dictionary<Organism, Habitat> overriddenChosenHabitats)
            : base(habitats, organismHabitats)
        {
            this.OverriddenDesiredHabitats = overriddenChosenHabitats;
        }

        protected override Dictionary<Organism, Habitat> GetDesiredOrganismHabitats()
        {
            return this.OverriddenDesiredHabitats;
        }

        protected override Organism DecideOrganism(List<Organism> organisms)
        {
            return organisms.First();
        }
    }
}
