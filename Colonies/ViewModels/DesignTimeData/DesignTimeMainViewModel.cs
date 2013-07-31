namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeMainViewModel : MainViewModel, IDesignTimeViewModel<Main>
    {
        private static readonly DesignTimeEcosystemViewModel SampleEcosystemViewModel = new DesignTimeEcosystemViewModel();
        private static readonly DesignTimeOrganismSynopsisViewModel SampleOrganismSynopsisViewModel = new DesignTimeOrganismSynopsisViewModel();

        public DesignTimeMainViewModel()
            : base(CreateDesignTimeMain(), SampleEcosystemViewModel, SampleOrganismSynopsisViewModel, new EventAggregator())
        {   
            
        }

        public Main DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Main CreateDesignTimeMain()
        {
            return new Main(SampleEcosystemViewModel.DesignTimeModel);
        }
    }
}
