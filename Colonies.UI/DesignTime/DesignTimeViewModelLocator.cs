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

        public static MainViewModel MainViewModel { get { return ViewModelBootstrapper.MainViewModel; } }

        public static EcosystemViewModel EcosystemViewModel { get { return MainViewModel.EcosystemViewModel; } }
        public static HabitatViewModel HabitatViewModel { get { return EcosystemViewModel.HabitatViewModels[1][1]; } }
        public static EnvironmentViewModel EnvironmentViewModel { get { return HabitatViewModel.EnvironmentViewModel; } }
        public static OrganismViewModel OrganismViewModel { get { return HabitatViewModel.OrganismViewModel; } }

        public static OrganismSynopsisViewModel OrganismSynopsisViewModel { get { return MainViewModel.OrganismSynopsisViewModel; } }
    }
}
