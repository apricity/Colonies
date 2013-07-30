namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeOrganismSummaryViewModel : OrganismSummaryViewModel, IDesignTimeViewModel<OrganismSummary>
    {
        private static readonly List<OrganismViewModel> SampleOrganismViewModels
            = new List<OrganismViewModel>
                {
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel(),
                    new DesignTimeOrganismViewModel()
                };

        public DesignTimeOrganismSummaryViewModel()
            : base(CreateDesignTimeOrganismSummary(), SampleOrganismViewModels, new EventAggregator())
        {

        }

        public OrganismSummary DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static OrganismSummary CreateDesignTimeOrganismSummary()
        {
            var organisms =
                SampleOrganismViewModels.Select(
                    organismViewModel => ((DesignTimeOrganismViewModel)organismViewModel).DesignTimeModel).ToList();

            return new OrganismSummary(organisms);
        }
    }
}
