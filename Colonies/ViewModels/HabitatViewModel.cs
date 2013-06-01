namespace Wacton.Colonies.ViewModels
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class HabitatViewModel : ViewModelBase<Habitat>
    {
        private EnvironmentViewModel environmentViewModel;
        public EnvironmentViewModel EnvironmentViewModel
        {
            get
            {
                return this.environmentViewModel;
            }
            set
            {
                this.environmentViewModel = value;
                this.OnPropertyChanged("EnvironmentViewModel");
            }
        }

        private OrganismViewModel organismViewModel;
        public OrganismViewModel OrganismViewModel
        {
            get
            {
                return this.organismViewModel;
            }
            set
            {
                this.organismViewModel = value;
                this.OnPropertyChanged("OrganismViewModel");
            }
        }

        public HabitatViewModel(Habitat domainModel, EnvironmentViewModel environmentViewModel, OrganismViewModel organismViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EnvironmentViewModel = environmentViewModel;
            this.OrganismViewModel = organismViewModel;
        }

        public void RefreshEnvironmentViewModel()
        {
            // TODO: this is MASSIVE overkill, surely...
            this.EnvironmentViewModel = new EnvironmentViewModel(this.DomainModel.Environment, this.EventAggregator);
            
            // this.EnvironmentViewModel = this.environmentViewModel;
            //this.OnPropertyChanged("PheromoneLevel");
            //this.OnPropertyChanged("EnvironmentViewModel");
        }

        public void RefreshOrganismViewModel()
        {
            // renew the organism view model (in case the model for this habitat is one that has changed)
            this.OrganismViewModel = new OrganismViewModel(this.DomainModel.Organism, this.EventAggregator);
        }
    }
}
