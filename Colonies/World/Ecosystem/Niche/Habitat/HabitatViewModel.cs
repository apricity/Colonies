using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public class HabitatViewModel : INotifyPropertyChanged
    {
        private readonly Habitat model;

        public Terrain Terrain
        {
            get
            {
                return this.model.Terrain;
            }
            set
            {
                this.model.Terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        public HabitatViewModel(Habitat model)
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
