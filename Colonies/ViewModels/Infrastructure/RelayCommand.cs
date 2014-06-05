namespace Wacton.Colonies.ViewModels.Infrastructure
{
    using System;
    using System.Windows.Input;

    class RelayCommand : ICommand
    {
        private readonly Action<object> executionLogic;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> executionLogic, Predicate<object> canExecute = null)
        {
            if (executionLogic == null)
            {
                throw new ArgumentNullException("executionLogic");
            }

            this.executionLogic = executionLogic;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            this.executionLogic(parameter);
        }

        public bool CanExecute(object parameter)
        {
            // if there is no "can execute" predicate, assume can execute
            return this.canExecute == null || this.canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
