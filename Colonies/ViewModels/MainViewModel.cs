namespace Colonies.ViewModels
{
    using System;
    using System.Threading;
    using System.Windows.Input;

    using Colonies.Events;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class MainViewModel : ViewModelBase<Main>
    {
        // if the timer interval is too small, the model update won't have finished
        // so use a lock to ensure the model isn't updated while it's updating...
        // (volatile because, if interval update is too small, lock will be accessed by multiple threads simultaneously)
        private Timer ecosystemTimer;
        private bool ecosystemRunning;
        private volatile object updateLock = new object();

        private EcosystemViewModel ecosystemViewModel;
        public EcosystemViewModel EcosystemViewModel
        {
            get
            {
                return this.ecosystemViewModel;
            }
            set
            {
                this.ecosystemViewModel = value;
                this.OnPropertyChanged("EcosystemViewModel");
            }
        }

        public ICommand StartEcosystemCommand { get; set; }

        public MainViewModel(Main model, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
            this.ecosystemRunning = false;

            // the "Start Ecosystem" button is bound to the StartEcosystemCommand
            // when the button is pressed, the Execute method is called, which in turn calls StartEcosystem
            this.StartEcosystemCommand = new RelayCommand(this.StartEcosystem, this.IsEcosystemRunning);
        }

        private void StartEcosystem(object parameter)
        {
            this.ecosystemRunning = true;
            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick, 1, 0, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        private bool IsEcosystemRunning(object parameter)
        {
            return !this.ecosystemRunning;
        }

        private void OnEcosystemTimerTick(object state)
        {
            lock (this.updateLock)
            {
                // update the ecosystem
                var turns = Convert.ToInt32(state);
                this.EcosystemViewModel.UpdateEcosystem(turns);
                this.EventAggregator.GetEvent<EcosystemTickEvent>().Publish(null); 
            }
        }
    }
}
