namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Collections.Generic;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleEcosystemViewModel : EcosystemViewModel
    {
        private static readonly List<List<HabitatViewModel>> SampleHabitatViewModels
            = new List<List<HabitatViewModel>>
                {
                    new List<HabitatViewModel> { new SampleHabitatViewModel(), new SampleHabitatViewModel() },
                    new List<HabitatViewModel> { new SampleHabitatViewModel(), new SampleHabitatViewModel() } 
                };

        public SampleEcosystemViewModel()
            : base(CreateSampleEcosystem(), SampleHabitatViewModels, new EventAggregator())
        {

        }

        public Ecosystem SampleEcosystem
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Ecosystem CreateSampleEcosystem()
        {
            var habitats = new Habitat[2,2];
            habitats[0, 0] = ((SampleHabitatViewModel)SampleHabitatViewModels[0][0]).SampleHabitat;
            habitats[0, 1] = ((SampleHabitatViewModel)SampleHabitatViewModels[0][1]).SampleHabitat;
            habitats[1, 0] = ((SampleHabitatViewModel)SampleHabitatViewModels[1][0]).SampleHabitat;
            habitats[1, 1] = ((SampleHabitatViewModel)SampleHabitatViewModels[1][1]).SampleHabitat;

            return new Ecosystem(habitats, new Dictionary<Organism, Habitat>());
        }
    }
}
