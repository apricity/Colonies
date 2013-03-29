namespace Colonies.ViewModels
{
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.Models;

    public sealed class OrganismViewModel : INotifyPropertyChanged
    {
        private Organism organismModel;
        public Organism OrganismModel
        {
            get
            {
                return this.organismModel;
            }
            set
            {
                this.organismModel = value;
                this.OnPropertyChanged("OrganismModel");
            }
        }

        public OrganismViewModel(Organism organismModel)
        {
            this.OrganismModel = organismModel;
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
