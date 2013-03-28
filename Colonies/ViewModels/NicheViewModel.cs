using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class NicheViewModel : INotifyPropertyChanged
    {
        private Niche nicheModel;
        public Niche NicheModel
        {
            get
            {
                return this.nicheModel;
            }
            set
            {
                this.nicheModel = value;
                this.OnPropertyChanged("NicheModel");
            }
        }

        private HabitatViewModel habitatViewModel;
        public HabitatViewModel HabitatViewModel
        {
            get
            {
                return this.habitatViewModel;
            }
            set
            {
                this.habitatViewModel = value;
                this.OnPropertyChanged("HabitatViewModel");
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

        public NicheViewModel(Niche model)
        {
            this.NicheModel = model;

            this.HabitatViewModel = new HabitatViewModel(this.NicheModel.Habitat);
            this.OrganismViewModel = new OrganismViewModel(this.NicheModel.Organism);
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
