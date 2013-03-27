using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public class EcosystemViewModel : INotifyPropertyChanged
    {
        private readonly Ecosystem model;

        public List<List<Niche>> Niches
        {
            get
            {
                return this.model.Niches;
            }
            set
            {
                this.model.Niches = value;
                this.OnPropertyChanged("Niches");
            }
        }

        public EcosystemViewModel(Ecosystem model)
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
