namespace Colonies.ViewModels
{
    using System;
    using System.Threading;

    using Colonies.Events;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class MainViewModel : ViewModelBase<Main>
    {
        // if the timer interval is too small, the model update won't have finished
        // so use a lock to ensure the model isn't updated while it's updating...
        // (volatile because, if interval update is too small, lock will be accessed by multiple threads simultaneously)
        private Timer ecosystemTimer;
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

        public MainViewModel(Main model, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
        }

        public void StartEcosystem()
        {
            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick, 1, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
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
