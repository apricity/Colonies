namespace Colonies.ViewModels
{
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class EnvironmentViewModel : ViewModelBase<Environment>
    {
        public Terrain Terrain
        {
            get
            {
                return this.DomainModel.Terrain;
            }
            set
            {
                this.DomainModel.Terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        public EnvironmentViewModel(Environment model, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {

        }
    }
}
