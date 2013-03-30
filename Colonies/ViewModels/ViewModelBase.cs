namespace Colonies.ViewModels
{
    using System.ComponentModel;

    using Colonies.Annotations;

    using Microsoft.Practices.Prism.Events;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged
    {
        public T DomainModel { get; set; }

        public IEventAggregator EventAggregator { get; private set; }

        protected ViewModelBase(T domainModel, IEventAggregator eventAggregator)
        {
            this.DomainModel = domainModel;
            this.EventAggregator = eventAggregator;
        }

        public override string ToString()
        {
            return this.DomainModel.ToString() + " (VM)";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
