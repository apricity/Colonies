namespace Colonies
{
    using System.ComponentModel;
    using System.Windows;

    using Colonies.Annotations;

    public sealed class Habitat : INotifyPropertyChanged
    {
        // TODO: a Habitat should not know its own coordinates
        public int X { get; private set; }
        public int Y { get; private set; }

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

        public Habitat()
        {
            this.Terrain = Terrain.Unknown;
        }

        public Habitat(int x, int y, Terrain terrain)
        {
            this.Terrain = terrain;
            this.X = x;
            this.Y = y;
        }
        
        public new string ToString()
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
