namespace Wacton.Tovarisch.MVVM
{
    using Caliburn.Micro;

    public abstract class ViewModelBase : PropertyChangedBase
    {
        public ViewModelBase(CommandInvoker commandInvoker, params object[] models)
        {
            this.CommandInvoker = commandInvoker;

            foreach (var model in models)
            {
                this.CommandInvoker.SubscribeToModelChange(model, this.UpdateForDomainModelChange);
            }
        }

        protected CommandInvoker CommandInvoker { get; private set; }

        protected virtual void UpdateForDomainModelChange()
        {
            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                this.NotifyOfPropertyChange(property.Name);
            }
        }
    }

    public abstract class ViewModelBase<T> : ViewModelBase
    {
        protected ViewModelBase(CommandInvoker commandInvoker, T model)
            : base(commandInvoker, model)
        {
        }
    }
}