namespace Wacton.Tovarisch.MVVM
{
    public abstract class DomainCommand<T>
    {
        private readonly CommandInvoker commandInvoker;
        private readonly object[] models;

        protected DomainCommand(CommandInvoker commandInvoker, params object[] models)
        {
            this.commandInvoker = commandInvoker;
            this.models = models;
        }

        public void ExecuteAndNotify(T parameter)
        {
            this.commandInvoker.ExecuteCommand(this.Execute, parameter, this.models, this.ToCommandString(parameter));
        }

        public void ExecuteAsyncAndNotify(T parameter)
        {
            this.commandInvoker.ExecuteCommandAsync(this.Execute, parameter, this.models, this.ToCommandString(parameter));
        }

        protected void NotifyAbout(object o)
        {
            this.commandInvoker.NotifyListenersAbout(o);
        }

        protected void NotifyAboutAll()
        {
            this.commandInvoker.NotifyListenersAbout(this.models);
        }

        protected abstract void Execute(T parameter);

        protected virtual string ToCommandString(T parameter)
        {
            return this.GetType().Name + ": " + parameter;
        }
    }

    public abstract class DomainCommand
    {
        private readonly CommandInvoker commandInvoker;
        private readonly object[] models;

        protected DomainCommand(CommandInvoker commandInvoker, params object[] models)
        {
            this.commandInvoker = commandInvoker;
            this.models = models;
        }

        protected abstract void Execute();

        public void ExecuteAndNotify()
        {
            this.commandInvoker.ExecuteCommand(this.Execute, this.models, this.ToCommandString());
        }

        public void ExecuteAsyncAndNotify()
        {
            this.commandInvoker.ExecuteCommandAsync(this.Execute, this.models, this.ToCommandString());
        }

        protected void NotifyAbout(object o)
        {
            this.commandInvoker.NotifyListenersAbout(o);
        }

        protected void NotifyAboutAll()
        {
            this.commandInvoker.NotifyListenersAbout(this.models);
        }

        protected virtual string ToCommandString()
        {
            return this.GetType().Name;
        }
    }
}