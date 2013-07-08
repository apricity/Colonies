namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeMainViewModel : MainViewModel, IDesignTimeViewModel<Main>
    {
        private static readonly DesignTimeEcosystemViewModel SampleEcosystemViewModel = new DesignTimeEcosystemViewModel();

        public DesignTimeMainViewModel()
            : base(CreateDesignTimeMain(), SampleEcosystemViewModel, new EventAggregator())
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
