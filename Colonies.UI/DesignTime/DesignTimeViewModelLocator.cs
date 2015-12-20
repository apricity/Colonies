namespace Wacton.Colonies.UI.DesignTime
{
    using Wacton.Colonies.UI.Ecosystems;
    using Wacton.Colonies.UI.Environments;
    using Wacton.Colonies.UI.Habitats;
    using Wacton.Colonies.UI.Mains;
    using Wacton.Colonies.UI.Organisms;
    using Wacton.Colonies.UI.OrganismSynopses;

    public class DesignTimeViewModelLocator
    {
        private static readonly DesignTimeDomainBootstrapper DomainBootstrapper;
        private static readonly DesignTimeViewModelBootstrapper ViewModelBootstrapper;

        static DesignTimeViewModelLocator()
        {
            if (ViewModelBootstrapper != null)
            {
                return;
            }

            DomainBootstrapper = new DesignTimeDomainBootstrapper();
            DomainBootstrapper.Run();

            ViewModelBootstrapper = new DesignTimeViewModelBootstrapper();
            ViewModelBootstrapper.Run(DomainBootstrapper.Domain);
        }

        public static MainViewModel MainViewModel => ViewModelBootstrapper.MainViewModel;

        public static EcosystemViewModel EcosystemViewModel => MainViewModel.EcosystemViewModel;
        public static HabitatViewModel HabitatViewModel => EcosystemViewModel.HabitatViewModels[0][0];
        public static EnvironmentViewModel EnvironmentViewModel => HabitatViewModel.EnvironmentViewModel;
        public static OrganismViewModel OrganismViewModel => HabitatViewModel.OrganismViewModel;

        public static OrganismSynopsisViewModel OrganismSynopsisViewModel => MainViewModel.OrganismSynopsisViewModel;
    }
}
