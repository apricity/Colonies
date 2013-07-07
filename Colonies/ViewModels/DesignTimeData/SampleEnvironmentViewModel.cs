namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleEnvironmentViewModel : EnvironmentViewModel
    {
        public SampleEnvironmentViewModel()
            : base(CreateSampleEnvironment(), new EventAggregator())
        {

        }

        public Environment SampleEnvironment
        {
            get
            {
                return this.DomainModel;                
            }
        }

        private static Environment CreateSampleEnvironment()
        {
            var environment = new Environment(Terrain.Unknown, true);
            environment.IncreasePheromoneLevel(0.5);
            return environment;
        }
    }
}
