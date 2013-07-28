namespace Wacton.Colonies.ViewModels
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public class OrganismViewModel : ViewModelBase<Organism>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        private Color color;
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                this.OnPropertyChanged("Color");
            }
        }

        private bool isAlive;
        public bool IsAlive
        {
            get
            {
                return this.isAlive;
            }
            set
            {
                this.isAlive = value;
                this.OnPropertyChanged("IsAlive");
            }
        }

        private double healthLevel;
        public double HealthLevel
        {
            get
            {
                return this.healthLevel;
            }
            set
            {
                this.healthLevel = value;
                this.OnPropertyChanged("HealthLevel");
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public OrganismViewModel(Organism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.Refresh();
        }

        // TODO: can Refresh() be a generic ViewModel method?
        public void Refresh()
        {
            if (this.DomainModel == null)
            {
                this.Color = Colors.Transparent;
                this.IsAlive = false;
                this.HealthLevel = 0;
                this.Name = string.Empty;
            }
            else
            {
                this.Color = this.DomainModel.Color;
                this.IsAlive = this.DomainModel.IsAlive;
                this.HealthLevel = this.DomainModel.Health.Level;
                this.Name = this.DomainModel.Name;
            }
        }

        public bool HasOrganism
        {
            get
            {
                return this.DomainModel != null;
            }
        }
    }
}
