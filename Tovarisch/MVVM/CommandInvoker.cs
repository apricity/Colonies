namespace Wacton.Tovarisch.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommandInvoker
        {
            private ModelChangeNotifier ModelChangeNotifier { get; set; }
            private readonly SynchronizationContext mainContext;

            public CommandInvoker(ModelChangeNotifier modelChangeNotifier)
            {
                this.ModelChangeNotifier = modelChangeNotifier;
                this.mainContext = SynchronizationContext.Current;
            }

            public void ExecuteCommandAsync<T>(Action<T> command, T parameter, IEnumerable<object> models, string commandName)
            {
                /* if there is a fault during the task action, post a task.Wait() onto the main thread
                 * task.Wait() will throw an AggregateException if exception was thrown during the execution of the Task
                 * because the Wait() is on the main thread, the thrown exception is will be caught by the unhandled exception handler */
                Task.Factory.StartNew(() => this.ExecuteCommand(command, parameter, models, commandName))
                    .ContinueWith(task => this.mainContext.Post(state => task.Wait(), null), TaskContinuationOptions.OnlyOnFaulted);
            }

            public void ExecuteCommand<T>(Action<T> command, T parameter, IEnumerable<object> models, string commandName)
            {
                this.ExecuteCommand(() => command(parameter), models, commandName);
            }

            public void ExecuteCommandAsync(Action command, IEnumerable<object> models, string commandName)
            {
                /* if there is a fault during the task action, post a task.Wait() onto the main thread
                 * task.Wait() will throw an AggregateException if exception was thrown during the execution of the Task
                 * because the Wait() is on the main thread, the thrown exception is will be caught by the unhandled exception handler */
                Task.Factory.StartNew(() => this.ExecuteCommand(command, models, commandName))
                    .ContinueWith(task => this.mainContext.Post(state => task.Wait(), null), TaskContinuationOptions.OnlyOnFaulted);
            }

            public void ExecuteCommand(Action command, IEnumerable<object> models, string commandName)
            {
                command();
                this.NotifyListenersAbout(models);
            }

            // added here to make the use of view models a little bit easier - but perhaps leads to a renaming of CommandInvoker
            public void SubscribeToModelChange(object objectToNotifyOn, Action actionOnChange)
            {
                this.ModelChangeNotifier.Subscribe(objectToNotifyOn, actionOnChange);
            }

            public void NotifyListenersAbout(IEnumerable<object> models)
            {
                foreach (var model in models)
                {
                    this.NotifyListenersAbout(model);
                }
            }

            public void NotifyListenersAbout(object model)
            {
                this.ModelChangeNotifier.Notify(model);
            }
        }
}
