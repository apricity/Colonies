namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Interfaces;

    public class OrganismSynopsisViewModel : ViewModelBase<IOrganismSynopsis>
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
                this.OnPropertyChanged("OrganismViewModels");
            }
        }

        public OrganismSynopsisViewModel(IOrganismSynopsis domainModel, List<OrganismViewModel> organismViewModels, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.OrganismViewModels = organismViewModels;
        }

        public override void Refresh()
        {
            // refresh all child view models (each organism)
            foreach (var organismViewModel in this.OrganismViewModels)
            {
                organismViewModel.Refresh();
            }
        }
    }
}
