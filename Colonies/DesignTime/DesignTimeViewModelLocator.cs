namespace Wacton.Colonies.DesignTime
{
    using Wacton.Colonies.ViewModels;

    public class DesignTimeViewModelLocator
    {
        private static readonly DesignTimeBootstrapper Bootstrapper;

        static DesignTimeViewModelLocator()
        {
            if (Bootstrapper != null)
            {
                return;
            }

            Bootstrapper = new DesignTimeBootstrapper();
            Bootstrapper.Run();
        }

        public static MainViewModel MainViewModel { get { return Bootstrapper.MainViewModel; } }

        public static EcosystemViewModel EcosystemViewModel { get { return MainViewModel.EcosystemViewModel; } }
        public static HabitatViewModel HabitatViewModel { get { return EcosystemViewModel.HabitatViewModels[0][0]; } }
        public static EnvironmentViewModel EnvironmentViewModel { get { return HabitatViewModel.EnvironmentViewModel; } }
        public static OrganismViewModel OrganismViewModel { get { return HabitatViewModel.OrganismViewModel; } }

        public static OrganismSynopsisViewModel OrganismSynopsisViewModel { get { return MainViewModel.OrganismSynopsisViewModel; } }
    }
}
