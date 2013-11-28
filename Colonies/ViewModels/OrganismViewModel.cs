namespace Wacton.Colonies.ViewModels
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public class OrganismViewModel : ViewModelBase<IOrganism>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        private bool hasOrganism;
        public bool HasOrganism
        {
            get
            {
                return this.hasOrganism;
            }
            set
            {
                this.hasOrganism = value;
                this.OnPropertyChanged("HasOrganism");
            }
        }

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

        public static double HabitatScale
        {
            get
            {
                return 0.5;
            }
        }

        
        public OrganismViewModel(IOrganism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }

        // TODO: can Refresh() be a generic ViewModel method?
        public override void Refresh()
        {
            if (this.DomainModel != null)
            {
                this.HasOrganism = true;
                this.Color = this.DomainModel.Color;
                this.IsAlive = this.DomainModel.IsAlive;
                this.HealthLevel = this.DomainModel.Measurement.GetLevel(Measure.Health);
                this.Name = this.DomainModel.Name;
            }
            else
            {
                // reset the properties to their default values if no organism in the model
                this.HasOrganism = false;
                this.Color = default(Color);
                this.IsAlive = default(bool);
                this.HealthLevel = default(double);
                this.Name = default(string); 
            }
        }
    }
}
