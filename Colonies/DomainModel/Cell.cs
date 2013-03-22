namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        private CellType cellType;
        public CellType CellType
        {
            get
            {
                return this.cellType;
            }
            set
            {
                this.cellType = value;
            }
        }

        private Occupant occupant;
        public Occupant Occupant
        {
            get
            {
                return this.occupant;
            }
            set
            {
                this.occupant = value;
            }
        }

        public bool HasOccupant
        {
            get
            {
                return this.Occupant != null;
            }
        }

        public Cell(CellType cellType, int x, int y)
        {
            this.cellType = cellType;
            this.X = x;
            this.Y = y;
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
        Unknown = -1,
        Earth,
        Grass,
        Water,
        Fire
    }
}
