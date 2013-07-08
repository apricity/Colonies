namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeHabitatViewModel : HabitatViewModel, IDesignTimeViewModel<Habitat>
    {
        private static readonly DesignTimeEnvironmentViewModel SampleEnvironmentViewModel = new DesignTimeEnvironmentViewModel();
        private static readonly DesignTimeOrganismViewModel SampleOrganismViewModel = new DesignTimeOrganismViewModel();

        public DesignTimeHabitatViewModel()
            : base(CreateDesignTimeHabitat(), SampleEnvironmentViewModel, SampleOrganismViewModel, new EventAggregator())
        {   
            
        }

        public Habitat DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Habitat CreateDesignTimeHabitat()
        {
            return new Habitat(SampleEnvironmentViewModel.DesignTimeModel, SampleOrganismViewModel.DesignTimeModel);
        }
    }
}
