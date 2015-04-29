namespace Wacton.Colonies
{
    using System.Reflection;

    using Wacton.Colonies.Domain;
    using Wacton.Colonies.Domain.Properties;
    using Wacton.Colonies.UI;

    public class Bootstrapper
    {
        public virtual void Run()
        {
            // get the version number to display on the main window title
            var assembly = Assembly.GetExecutingAssembly();
            var version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();

            // create the view to display to the user
            // the data context is the view model tree that contains the model
            var domainBootstrapper = new DomainBootstrapper();
            var domainModel = domainBootstrapper.BuildDomainModel(Settings.Default.EcosystemWidth, Settings.Default.EcosystemHeight);

            var viewModelBootstrapper = new ViewModelBootstrapper();
            var viewModel = viewModelBootstrapper.BuildViewModel(domainModel);
            viewModel.Refresh();

            var mainView = new MainView { DataContext = viewModel };
            mainView.Title += string.Format(" ({0})", version);

            // display the window to the user!
            mainView.Show();
        }
    }
}
