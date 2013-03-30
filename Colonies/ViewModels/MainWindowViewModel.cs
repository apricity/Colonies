namespace Colonies.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using Colonies.Annotations;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class MainWindowViewModel : ViewModelBase<MainWindow>
    {
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

        public MainWindowViewModel(MainWindow model, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
        }

        public void StartEcosystem()
        {
            this.ecosystemTimer = new Timer(this.UpdateEcosystem, null, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        private void UpdateEcosystem(object state)
        {
            lock (this.updateLock)
            {
                this.EcosystemViewModel.DomainModel.Update();
                this.EventAggregator.GetEvent<OrganismMovedEvent>().Publish(String.Empty); 
            }
        }
    }

    public class OrganismMovedEvent : CompositePresentationEvent<string>
    {
    }
}
