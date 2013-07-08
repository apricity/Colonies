namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeEnvironmentViewModel : EnvironmentViewModel, IDesignTimeViewModel<Environment>
    {
        public DesignTimeEnvironmentViewModel()
            : base(CreateDesignTimeEnvironment(), new EventAggregator())
        {

        }

        public Environment DesignTimeModel
        {
            get
            {
                return this.DomainModel;                
            }
        }

        private static Environment CreateDesignTimeEnvironment()
        {
            var environment = new Environment(Terrain.Unknown, true);
            environment.IncreasePheromoneLevel(0.5);
            return environment;
        }
    }
}
