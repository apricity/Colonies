namespace Colonies.ViewModels
{
    using System.Windows.Media;

    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class EnvironmentViewModel : ViewModelBase<Environment>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        public Terrain Terrain
        {
            get
            {
                return this.DomainModel.Terrain;
            }
        }

        public double PheromoneOpacity
        {
            get
            {
                return this.DomainModel.PheromoneLevel;
            }
        }

        public EnvironmentViewModel(Environment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }
    }
}
