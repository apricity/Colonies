namespace Wacton.Colonies.UI.Infrastructure
{
    using System;
    using System.ComponentModel;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Properties;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : class
    {
        protected T DomainModel { get; private set; }
        protected IEventAggregator EventAggregator { get; private set; }

        protected ViewModelBase(T domainModel, IEventAggregator eventAggregator)
        {
            this.DomainModel = domainModel;
            this.EventAggregator = eventAggregator;
        }

        public abstract void Refresh();

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
