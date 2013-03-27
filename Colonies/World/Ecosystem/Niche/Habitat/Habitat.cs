namespace Colonies
{
    using System.ComponentModel;
    using System.Windows;

    using Colonies.Annotations;

    public sealed class Habitat : INotifyPropertyChanged
    {
        private Terrain terrain;
        public Terrain Terrain
        {
            get
            {
                return this.terrain;
            }
            set
            {
                this.terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        public Habitat(Terrain terrain)
        {
            this.Terrain = terrain;
        }
        
        public override string ToString()
        {
            return this.Terrain.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
