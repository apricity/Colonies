namespace Colonies.ViewModels
{
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.Models;

    public sealed class HabitatViewModel : INotifyPropertyChanged
    {
        private Habitat habitatModel;
        public Habitat HabitatModel
        {
            get
            {
                return this.habitatModel;
            }
            set
            {
                this.habitatModel = value;
                this.OnPropertyChanged("HabitatModel");
            }
        }

        private EnvironmentViewModel environmentViewModel;
        public EnvironmentViewModel EnvironmentViewModel
        {
            get
            {
                return this.environmentViewModel;
            }
            set
            {
                this.environmentViewModel = value;
                this.OnPropertyChanged("EnvironmentViewModel");
            }
        }

        private OrganismViewModel organismViewModel;
        public OrganismViewModel OrganismViewModel
        {
            get
            {
                return this.organismViewModel;
            }
            set
            {
                this.organismViewModel = value;
                this.OnPropertyChanged("OrganismViewModel");
            }
        }

        public HabitatViewModel(Habitat model)
        {
            this.HabitatModel = model;

            this.EnvironmentViewModel = new EnvironmentViewModel(this.HabitatModel.Environment);
            this.OrganismViewModel = new OrganismViewModel(this.HabitatModel.Organism);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
