namespace Colonies.ViewModels
{
    using System.Drawing;

    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class OrganismViewModel : ViewModelBase<Organism>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        public Color Color
        {
            get
            {
                return !this.HasOrganism() ? Color.Empty : this.DomainModel.Color;
            }
        }

        public double Opacity
        {
            get
            {
                return !this.HasOrganism() ? 0 : 1;
            }
        }

        public OrganismViewModel(Organism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }

        private bool HasOrganism()
        {
            return this.DomainModel != null;
        }
    }
}
