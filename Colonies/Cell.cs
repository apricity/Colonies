namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class Cell : INotifyPropertyChanged
    {
        private CellType cellType;

        public Cell(CellType cellType)
        {
            this.cellType = cellType;
        }

        public CellType CellType
        {
            get
            {
                return this.cellType;
            }
            set
            {
                this.cellType = value;
                this.OnPropertyChanged("CellType");
            }
        }

        public new string ToString()
        {
            return this.CellType.ToString();
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

    public enum CellType
    {
        Unknown,
        Earth,
        Grass,
        Water,
        Fire
    }
}
