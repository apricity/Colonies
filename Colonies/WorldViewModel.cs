namespace Colonies
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Colonies.Annotations;

    public class WorldViewModel : INotifyPropertyChanged
    {
        private int height;
        private int width;

        private ObservableCollection<Cell> cells = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> Cells
        {
            get
            {
                return this.cells;
            }
        }

        public WorldViewModel()
        {
            this.height = 4;
            this.width = 4;

            for (int i = 0; i < this.height * this.width; i++)
            {
                this.cells.Add(new Cell(i));
            }
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
