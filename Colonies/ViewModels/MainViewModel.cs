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
        private readonly Timer ecosystemTimer;
        private readonly TimeSpan infiniteTimeSpan = TimeSpan.FromMilliseconds(-1);
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

                if (this.isEcosystemRunning)
                {
                    this.UpdateEcosystemTurnInterval(true);
                }
                else
                {
                    this.UpdateEcosystemTurnInterval(false);
                }
            }
        }

        private double currentEcosystemFrequency;
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

        public MainViewModel(Main model, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
            this.isEcosystemRunning = false;

            this.ecosystemFrequency = Properties.Settings.Default.UpdateFrequencyInMs;
            const int ecosystemTurnsPerTick = 1;

            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick, ecosystemTurnsPerTick, this.infiniteTimeSpan, this.infiniteTimeSpan);
        }

        private void UpdateEcosystemTurnInterval(bool isActive)
        {
            const int immediateStart = 0;
            const int preventStart = Timeout.Infinite;

            this.ecosystemTimer.Change(isActive ? immediateStart : preventStart, (int)(this.EcosystemFrequency + 1) * 10);
            this.currentEcosystemFrequency = this.EcosystemFrequency;
        }

        private void OnEcosystemTimerTick(object state)
        {
            lock (this.updateLock)
            {
                // update the ecosystem
                var turns = Convert.ToInt32(state);
                this.EcosystemViewModel.UpdateEcosystem(turns);
                this.EventAggregator.GetEvent<EcosystemTickEvent>().Publish(null);

                if (this.currentEcosystemFrequency != this.EcosystemFrequency)
                {
                    this.UpdateEcosystemTurnInterval(true);
                }
            }
        }
    }
}
