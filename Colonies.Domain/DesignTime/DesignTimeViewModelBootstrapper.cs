namespace Wacton.Colonies.Domain.DesignTime
{
    using Wacton.Colonies.Domain.Main;

    public class DesignTimeViewModelBootstrapper : ViewModelBootstrapper
    {
        public MainViewModel MainViewModel { get; private set; }

        public void Run(Main domainModel)
        {
            this.MainViewModel = this.BuildViewModel(domainModel);
            this.MainViewModel.Refresh();
        }
    }
}
