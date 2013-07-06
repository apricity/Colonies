namespace Wacton.Colonies.ViewModels
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class OrganismViewModel : ViewModelBase<Organism>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        public Color Color
        {
            get
            {
                // TODO: how to handle !this.HasOrganism
                return !this.HasOrganism ? Colors.Transparent : this.DomainModel.Color;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.HasOrganism && this.DomainModel.IsAlive;
            }
        }

        public double HealthLevel
        {
            get
            {
                return !this.HasOrganism ? 0 : this.DomainModel.Health.Level;
            }
        }

        public OrganismViewModel(Organism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

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
