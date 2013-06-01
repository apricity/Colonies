namespace Wacton.Colonies.ViewModels
{
    using System.ComponentModel;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Properties;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged
    {
        protected T DomainModel { get; private set; }
        protected IEventAggregator EventAggregator { get; private set; }

        protected ViewModelBase(T domainModel, IEventAggregator eventAggregator)
        {
            this.DomainModel = domainModel;
            this.EventAggregator = eventAggregator;
        }

        public override string ToString()
        {
            return string.Format("VM: {0}", this.DomainModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
