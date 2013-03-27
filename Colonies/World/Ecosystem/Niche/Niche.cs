using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class Niche : INotifyPropertyChanged
    {
        private Habitat habitat;
        public Habitat Habitat
        {
            get
            {
                return this.habitat;
            }
            set
            {
                this.habitat = value;
                this.OnPropertyChanged("Habitat");
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
            }
        }

        public Niche(Habitat habitat, Organism organism)
        {
            this.Habitat = habitat;
            this.Organism = organism;
        }

        public override String ToString()
        {
            return String.Format("{0}, {1}", this.Habitat, this.Organism);
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
