namespace Colonies.ViewModels
{
    using System.ComponentModel;
    using System.Windows;

    using Colonies.Annotations;
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

        private OrganismViewModel OrganismViewModel { get; set; }

        private Visibility organismVisibility;
        public Visibility OrganismVisibility
        {
            get
            {
                return this.organismVisibility;
            }
            set
            {
                this.organismVisibility = value;
                this.OnPropertyChanged("OrganismVisibility");
            }
        }

        public HabitatViewModel(Habitat model, EnvironmentViewModel environmentViewModel, OrganismViewModel organismViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EventAggregator.GetEvent<OrganismMovedEvent>().Subscribe(this.UpdateVisibility);

            this.EnvironmentViewModel = environmentViewModel;
            this.OrganismViewModel = organismViewModel;
        }

        private void UpdateVisibility(string s)
        {
            if (this.DomainModel.ContainsOrganism())
            {
                this.OrganismVisibility = Visibility.Visible;
            }
            else
            {
                this.OrganismVisibility = Visibility.Hidden;
            }
        }
    }
}
