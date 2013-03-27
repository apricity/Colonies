using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class TileViewModel : INotifyPropertyChanged
    {
        private Habitat currentHabitat;
        public Habitat CurrentHabitat
        {
            get
            {
                return this.currentHabitat;
            }
            set
            {
                this.currentHabitat = value;
                this.OnPropertyChanged("CurrentHabitat");
            }
        }

        private Organism organism;
        public Organism Organism
        {
            get
            {
                return this.organism;
            }
            set
            {
                this.organism = value;
                this.OnPropertyChanged("Organism");

                this.ContainsOrganism = (value != null);
            }
        }

        private bool containsOrganism;
        public bool ContainsOrganism
        {
            get
            {
                return this.containsOrganism;
            }
            private set
            {
                this.containsOrganism = value;
                this.OnPropertyChanged("ContainsOrganism");
            }
        }

        public new string ToString()
        {
            return this.CurrentHabitat.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
