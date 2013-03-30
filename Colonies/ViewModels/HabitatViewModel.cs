namespace Colonies.ViewModels
{
    using System.Drawing;

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

        private Color organismColor;
        public Color OrganismColor
        {
            get
            {
                return this.organismColor;
            }
            set
            {
                this.organismColor = value;
                this.OnPropertyChanged("OrganismColor");
            }
        }

        private double organismOpacity;
        public double OrganismOpacity
        {
            get
            {
                return this.organismOpacity;
            }
            set
            {
                this.organismOpacity = value;
                this.OnPropertyChanged("OrganismOpacity");
            }
        }

        public HabitatViewModel(Habitat model, EnvironmentViewModel environmentViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EventAggregator.GetEvent<OrganismMovedEvent>().Subscribe(this.UpdateOrganism);

            this.EnvironmentViewModel = environmentViewModel;
        }

        private void UpdateOrganism(string s)
        {
            // when the event aggregator informs us that an organism has moved
            // update organism display values accordingly
            // TODO: pass the habitats that have been moved from and to, so we only update habitats that have been modified
            if (this.DomainModel.ContainsOrganism())
            {
                this.OrganismColor = this.DomainModel.Organism.Color;
                this.OrganismOpacity = 1;
            }
            else
            {
                this.OrganismColor = Color.Empty;
                this.OrganismOpacity = 0;
            }
        }
    }
}
