namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public class Cell : INotifyPropertyChanged
    {
        private int cellID;

        public Cell(int cellID)
        {
            this.cellID = cellID;
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
