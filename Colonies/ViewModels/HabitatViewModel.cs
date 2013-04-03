namespace Colonies.ViewModels
{
    using System;
    using System.Drawing;

    using Colonies.Events;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

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
        
        public HabitatViewModel(Habitat model, EnvironmentViewModel environmentViewModel, OrganismViewModel organismViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EnvironmentViewModel = environmentViewModel;
            this.OrganismViewModel = organismViewModel;

            // subscribe to the ecosystem tick event, so the habitat can refresh its view of the organism each turn
            this.EventAggregator.GetEvent<EcosystemTickEvent>().Subscribe(this.RefreshOrganismViewModel);
        }

        private void RefreshOrganismViewModel(object payload)
        {
            // TODO: should the event aggregator inform only the habitat view models with organism changes
            // TODO: or is forcing ALL the habitat view models to update an acceptable thing to do?
            // when the event aggregator informs us that organism have changed habitats
            // renew the organism view model (in case the model for this habitat is one that has changed)
            this.OrganismViewModel = new OrganismViewModel(this.DomainModel.Organism, this.EventAggregator);
        }
    }
}
