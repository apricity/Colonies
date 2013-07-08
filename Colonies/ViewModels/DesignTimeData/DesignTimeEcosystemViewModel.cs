namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Collections.Generic;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeEcosystemViewModel : EcosystemViewModel, IDesignTimeViewModel<Ecosystem>
    {
        private static readonly List<List<HabitatViewModel>> SampleHabitatViewModels
            = new List<List<HabitatViewModel>>
                {
                    new List<HabitatViewModel> { new DesignTimeHabitatViewModel(), new DesignTimeHabitatViewModel() },
                    new List<HabitatViewModel> { new DesignTimeHabitatViewModel(), new DesignTimeHabitatViewModel() } 
                };

        public DesignTimeEcosystemViewModel()
            : base(CreateDesignTimeEcosystem(), SampleHabitatViewModels, new EventAggregator())
        {

        }

        public Ecosystem DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Ecosystem CreateDesignTimeEcosystem()
        {
            var habitats = new Habitat[2,2];
            habitats[0, 0] = ((DesignTimeHabitatViewModel)SampleHabitatViewModels[0][0]).DesignTimeModel;
            habitats[0, 1] = ((DesignTimeHabitatViewModel)SampleHabitatViewModels[0][1]).DesignTimeModel;
            habitats[1, 0] = ((DesignTimeHabitatViewModel)SampleHabitatViewModels[1][0]).DesignTimeModel;
            habitats[1, 1] = ((DesignTimeHabitatViewModel)SampleHabitatViewModels[1][1]).DesignTimeModel;

            return new Ecosystem(habitats, new Dictionary<Organism, Habitat>());
        }
    }
}
