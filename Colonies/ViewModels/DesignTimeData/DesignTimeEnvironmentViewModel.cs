namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;
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
            var environment = new Environment(Terrain.Earth);
            environment.SetLevel(Measure.Obstruction, 1.0);
            environment.SetLevel(Measure.Pheromone, 0.5);
            environment.SetLevel(Measure.Nutrient, 0.5);
            environment.SetLevel(Measure.Mineral, 0.5);
            return environment;
        }
    }
}
