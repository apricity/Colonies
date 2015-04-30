namespace Wacton.Colonies.UI.DesignTime
{
    using Wacton.Colonies.Domain.Mains;
    using Wacton.Colonies.UI.Mains;

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
