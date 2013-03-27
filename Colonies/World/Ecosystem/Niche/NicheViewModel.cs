using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public class NicheViewModel : INotifyPropertyChanged
    {
        private readonly Niche model;

        public Habitat Habitat
        {
            get
            {
                return this.model.Habitat;
            }
            set
            {
                this.model.Habitat = value;
                this.OnPropertyChanged("Habitat");
            }
        }

        public Organism Organism
        {
            get
            {
                return this.model.Organism;
            }
            set
            {
                this.model.Organism = value;
                this.OnPropertyChanged("Organism");
            }
        }

        public NicheViewModel(Niche model)
        {
            this.model = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
