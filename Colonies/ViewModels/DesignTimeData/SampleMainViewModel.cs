namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleMainViewModel : MainViewModel
    {
        private static readonly SampleEcosystemViewModel SampleEcosystemViewModel = new SampleEcosystemViewModel();

        public SampleMainViewModel()
            : base(CreateSampleMain(), SampleEcosystemViewModel, new EventAggregator())
        {   
            
        }

        public Main SampleMain
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Main CreateSampleMain()
        {
            return new Main(SampleEcosystemViewModel.SampleEcosystem);
        }
    }
}
