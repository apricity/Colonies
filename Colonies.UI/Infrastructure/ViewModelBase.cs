namespace Wacton.Colonies.UI.Infrastructure
{
    using System;
    using System.ComponentModel;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Properties;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : class
    {
        protected T DomainModel { get; }
        protected IEventAggregator EventAggregator { get; private set; }

        protected ViewModelBase(T domainModel, IEventAggregator eventAggregator)
        {
            this.DomainModel = domainModel;
            this.EventAggregator = eventAggregator;
        }

        public abstract void Refresh();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"VM: {this.DomainModel}";
    }
}
