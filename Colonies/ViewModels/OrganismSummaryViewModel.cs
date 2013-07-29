namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class OrganismSummaryViewModel : ViewModelBase<Ecosystem>
    {
        private List<OrganismViewModel> organismViewModels;
        public List<OrganismViewModel> OrganismViewModels
        {
            get
            {
                return this.organismViewModels;
            }
            set
            {
                this.organismViewModels = value;
                this.OnPropertyChanged("organismViewModels");
            }
        }

        public OrganismSummaryViewModel(Ecosystem domainModel, List<OrganismViewModel> organismViewModels, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.OrganismViewModels = organismViewModels;
        }

        public override void Refresh()
        {
            // to complete when there is something to refresh
        }
    }
}
