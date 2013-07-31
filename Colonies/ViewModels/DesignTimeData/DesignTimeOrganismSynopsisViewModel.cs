namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeOrganismSynopsisViewModel : OrganismSynopsisViewModel, IDesignTimeViewModel<OrganismSynopsis>
    {
        private static readonly List<OrganismViewModel> SampleOrganismViewModels
            = new List<OrganismViewModel>
                {
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel()
                };

        public DesignTimeOrganismSynopsisViewModel()
            : base(CreateDesignTimeOrganismSynopsis(), SampleOrganismViewModels, new EventAggregator())
        {

        }

        public OrganismSynopsis DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static OrganismSynopsis CreateDesignTimeOrganismSynopsis()
        {
            var organisms =
                SampleOrganismViewModels.Select(
                    organismViewModel => ((DesignTimeOrganismViewModel)organismViewModel).DesignTimeModel).ToList();

            return new OrganismSynopsis(organisms);
        }
    }
}
