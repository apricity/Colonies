namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleHabitatViewModel : HabitatViewModel
    {
        private static readonly SampleEnvironmentViewModel SampleEnvironmentViewModel = new SampleEnvironmentViewModel();
        private static readonly SampleOrganismViewModel SampleOrganismViewModel = new SampleOrganismViewModel();

        public SampleHabitatViewModel()
            : base(CreateSampleHabitat(), SampleEnvironmentViewModel, SampleOrganismViewModel, new EventAggregator())
        {   
            
        }

        public Habitat SampleHabitat
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Habitat CreateSampleHabitat()
        {
            return new Habitat(SampleEnvironmentViewModel.SampleEnvironment, SampleOrganismViewModel.SampleOrganism);
        }
    }
}
