namespace Colonies.ViewModels
{
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

        public EnvironmentViewModel(Environment model, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {

        }
    }
}
