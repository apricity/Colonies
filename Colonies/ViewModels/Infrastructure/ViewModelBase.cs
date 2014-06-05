namespace Wacton.Colonies.ViewModels.Infrastructure
{
    using System;
    using System.ComponentModel;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Properties;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : class
    {
        protected T DomainModel { get; private set; }
        protected IEventAggregator EventAggregator { get; private set; }

        protected ViewModelBase(T domainModel, IEventAggregator eventAggregator)
        {
            this.DomainModel = domainModel;
            this.EventAggregator = eventAggregator;
        }

        public void AssignModel(T model)
        {
            if (this.DomainModel != null)
            {
                throw new InvalidOperationException("Cannot assign a model when a model is already present");
            }

            if (model == null)
            {
                throw new InvalidOperationException("Cannot assign a null model");
            }

            this.DomainModel = model;
            this.Refresh();
        }

        public void UnassignModel()
        {
            if (this.DomainModel == null)
            {
                throw new InvalidOperationException("Cannot unassign a model when no model is present");
            }

            this.DomainModel = default(T);
            this.Refresh();
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
