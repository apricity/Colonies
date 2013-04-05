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
                if (!this.HasOrganism())
                {
                    return Color.Empty;
                }

                return this.DomainModel.Color;
            }
        }

        public double Opacity
        {
            get
            {
                if (!this.HasOrganism())
                {
                    return 0;
                }

                return 1;
            }
        }

        public OrganismViewModel(Organism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }

        public bool HasOrganism()
        {
            return this.DomainModel != null;
        }
    }
}
