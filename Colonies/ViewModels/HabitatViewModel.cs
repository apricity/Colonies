using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

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

        public Terrain Terrain
        {
            get
            {
                return this.HabitatModel.Terrain;
            }
            set
            {
                this.HabitatModel.Terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        public HabitatViewModel(Habitat model)
        {
            this.HabitatModel = model;
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
