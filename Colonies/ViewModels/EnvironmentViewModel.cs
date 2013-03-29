namespace Colonies.ViewModels
{
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.Models;

    public sealed class EnvironmentViewModel : INotifyPropertyChanged
    {
        private Environment environmentModel;
        public Environment EnvironmentModel
        {
            get
            {
                return this.environmentModel;
            }
            set
            {
                this.environmentModel = value;
                this.OnPropertyChanged("EnvironmentModel");
            }
        }

        public Terrain Terrain
        {
            get
            {
                return this.EnvironmentModel.Terrain;
            }
            set
            {
                this.EnvironmentModel.Terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        public EnvironmentViewModel(Environment model)
        {
            this.EnvironmentModel = model;
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
