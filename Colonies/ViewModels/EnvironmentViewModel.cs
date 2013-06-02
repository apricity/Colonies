namespace Wacton.Colonies.ViewModels
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

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

        public bool IsObstructed
        {
            get
            {
                return this.DomainModel.IsObstructed;
            }
        }

        public double PheromoneOpacity
        {
            get
            {
                return this.DomainModel.Pheromone.Level;
            }
        }

        public EnvironmentViewModel(Environment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }
    }
}
