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
        private TimeSpan infiniteTimeSpan = TimeSpan.FromMilliseconds(-1);
        private volatile object updateLock = new object();

        public ICommand ToggleEcosystemCommand { get; set; }

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

        private bool isEcosystemRunning;
        public bool IsEcosystemRunning
        {
            get
            {
                return this.isEcosystemRunning;
            }
            set
            {
                this.isEcosystemRunning = value;
                this.OnPropertyChanged("IsEcosystemRunning");

                this.UpdateEcosystemRunning();
            }
        }

        private TimeSpan currentEcosystemFrequency;
        private double ecosystemFrequency;
        public double EcosystemFrequency
        {
            get
            {
                return this.ecosystemFrequency;
            }
            set
            {
                this.ecosystemFrequency = value;
                this.OnPropertyChanged("EcosystemFrequency");
            }
        }

        private TimeSpan EcosystemFrequencyTimeSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds((this.EcosystemFrequency + 1) * 10);
            }
        }

        public MainViewModel(Main model, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
            this.isEcosystemRunning = false;

            this.ecosystemFrequency = Properties.Settings.Default.UpdateFrequencyInMs;
            const int ecosystemTurnsPerTick = 1;

            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick, ecosystemTurnsPerTick, this.infiniteTimeSpan, this.infiniteTimeSpan);

            // the "Start Ecosystem" button is bound to the ToggleEcosystemCommand
            // when the button is pressed, the Execute method is called, which in turn calls StartEcosystem
            // this.ToggleEcosystemCommand = new RelayCommand(this.StartEcosystem, this.IsEcosystemRunning);
            this.ToggleEcosystemCommand = new RelayCommand(this.ToggleEcosystem);
        }

        private void ToggleEcosystem(object parameter)
        {
            if (this.isEcosystemRunning)
            {
                this.ecosystemTimer.Change(this.infiniteTimeSpan, this.infiniteTimeSpan);
                this.currentEcosystemFrequency = this.infiniteTimeSpan;
            }
            else
            {
                this.ecosystemTimer.Change(TimeSpan.FromMilliseconds(0), this.EcosystemFrequencyTimeSpan);
                this.currentEcosystemFrequency = this.EcosystemFrequencyTimeSpan;
            }

            // ecosystem is now the opposite running state
            this.isEcosystemRunning = !this.isEcosystemRunning;
        }

        private void UpdateEcosystemRunning()
        {
            if (this.isEcosystemRunning)
            {
                this.ecosystemTimer.Change(TimeSpan.FromMilliseconds(0), this.EcosystemFrequencyTimeSpan);
                this.currentEcosystemFrequency = this.EcosystemFrequencyTimeSpan;
            }
            else
            {
                this.ecosystemTimer.Change(this.infiniteTimeSpan, this.infiniteTimeSpan);
                this.currentEcosystemFrequency = this.infiniteTimeSpan;
            }
        }

        private void UpdateEcosystemFrequency()
        {
        }

        private void StartEcosystem(object parameter)
        {
            this.isEcosystemRunning = true;
            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick, 1, 0, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        //private bool IsEcosystemRunning(object parameter)
        //{
        //    return !this.isEcosystemRunning;
        //}

        private void OnEcosystemTimerTick(object state)
        {
            lock (this.updateLock)
            {
                // update the ecosystem
                var turns = Convert.ToInt32(state);
                this.EcosystemViewModel.UpdateEcosystem(turns);
                this.EventAggregator.GetEvent<EcosystemTickEvent>().Publish(null);

                if (this.currentEcosystemFrequency != this.EcosystemFrequencyTimeSpan)
                {
                    this.ecosystemTimer.Change(TimeSpan.FromMilliseconds(0), this.EcosystemFrequencyTimeSpan);
                    this.currentEcosystemFrequency = this.EcosystemFrequencyTimeSpan;
                }
            }
        }
    }
}
