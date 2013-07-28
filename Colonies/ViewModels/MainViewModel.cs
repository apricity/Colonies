namespace Wacton.Colonies.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public class MainViewModel : ViewModelBase<Main>
    {
        // if the timer interval is too small, the model update won't have finished
        // so use a lock to ensure the model isn't updated while it's updating...
        // (volatile because, if interval update is too small, lock will be accessed by multiple threads simultaneously)
        private readonly Timer ecosystemTimer;
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

        private OrganismSummaryViewModel organismSummaryViewModel;
        public OrganismSummaryViewModel OrganismSummaryViewModel
        {
            get
            {
                return this.organismSummaryViewModel;
            }
            set
            {
                this.organismSummaryViewModel = value;
                this.OnPropertyChanged("OrganismSummaryViewModel");
            }
        }

        private bool isEcosystemActive;
        public bool IsEcosystemActive
        {
            get
            {
                return this.isEcosystemActive;
            }
            set
            {
                this.isEcosystemActive = value;
                this.OnPropertyChanged("IsEcosystemActive");

                // if the ecosystem turns on/off the timer needs to start/stop 
                this.ChangeEcosystemTimer();
            }
        }

        // TODO: should the slider go from "slow (1) -> fast (100)", and that value be converted in this view model to a ms value?
        // turn interval is in ms
        private int ecosystemTurnInterval;
        public int EcosystemTurnInterval
        {
            get
            {
                return this.ecosystemTurnInterval;
            }
            set
            {
                this.ecosystemTurnInterval = value;
                this.OnPropertyChanged("EcosystemTurnInterval");
            }
        }

        private int lastUsedTurnInterval;

        public double PheromoneDepositPerTurn
        {
            get
            {
                return this.DomainModel.Ecosystem.PheromoneDepositPerTurn;
            }
            set
            {
                this.DomainModel.Ecosystem.PheromoneDepositPerTurn = value;
                this.OnPropertyChanged("PheromoneDepositPerTurn");
            }
        }

        public double PheromoneFadePerTurn
        {
            get
            {
                return this.DomainModel.Ecosystem.PheromoneFadePerTurn;
            }
            set
            {
                this.DomainModel.Ecosystem.PheromoneFadePerTurn = value;
                this.OnPropertyChanged("PheromoneFadePerTurn");
            }
        }

        public MainViewModel(Main domainModel, EcosystemViewModel ecosystemViewModel, OrganismSummaryViewModel organismSummaryViewModel, IEventAggregator eventAggregator)
            : this(domainModel, ecosystemViewModel, eventAggregator)
        {
            this.OrganismSummaryViewModel = organismSummaryViewModel;
        }

        public MainViewModel(Main domainModel, EcosystemViewModel ecosystemViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;

            // initally set the ecosystem up to be not running
            this.ecosystemTimer = new Timer(this.OnEcosystemTimerTick);
            this.IsEcosystemActive = false;
            var initialUpdateInterval = Properties.Settings.Default.UpdateFrequencyInMs;
            this.EcosystemTurnInterval = initialUpdateInterval;
            this.lastUsedTurnInterval = initialUpdateInterval;

            // hook up a toggle ecosystem command so a keyboard shortcut can be used to toggle the ecosystem on/off
            this.ToggleEcosystemCommand = new RelayCommand(this.ToggleEcosystem);

        }

        private void ToggleEcosystem(object obj)
        {
            this.IsEcosystemActive = !this.IsEcosystemActive;
        }

        private void ChangeEcosystemTimer()
        {
            const int immediateStart = 0;
            const int preventStart = Timeout.Infinite;

            this.ecosystemTimer.Change(this.IsEcosystemActive ? immediateStart : preventStart, this.EcosystemTurnInterval);
            this.lastUsedTurnInterval = this.EcosystemTurnInterval;
        }

        private void OnEcosystemTimerTick(object state)
        {
            lock (this.updateLock)
            {
                var updateSummary = this.DomainModel.UpdateOnce();
                this.UpdateViewModels(updateSummary);

                //this.EcosystemViewModel.ProgressEcosystemOneTurn();
                
                // if there's been a change in the turn interval while the previous turn was processed
                // update the interval of the ecosystem timer
                if (this.EcosystemTurnInterval != this.lastUsedTurnInterval)
                {
                    this.ChangeEcosystemTimer();
                }
            }
        }

        private void UpdateViewModels(UpdateSummary updateSummary)
        {
            // ecosystem updates
            foreach (var preUpdateOrganismLocation in updateSummary.PreUpdateOrganismLocations)
            {
                var x = preUpdateOrganismLocation.Value.X;
                var y = preUpdateOrganismLocation.Value.Y;
                var organism = this.DomainModel.Ecosystem.Habitats[x, y].Organism;
                this.EcosystemViewModel.HabitatViewModels[x][y].OrganismViewModel = new OrganismViewModel(null, this.EventAggregator);
            }

            foreach (var postUpdateOrganismLocation in updateSummary.PostUpdateOrganismLocations)
            {
                var x = postUpdateOrganismLocation.Value.X;
                var y = postUpdateOrganismLocation.Value.Y;
                var organism = this.DomainModel.Ecosystem.Habitats[x, y].Organism;
                this.EcosystemViewModel.HabitatViewModels[x][y].OrganismViewModel = new OrganismViewModel(organism, this.EventAggregator);
            }

            var pheromoneChangedLocations =
                updateSummary.PheromoneDecreasedLocations.Union(
                    updateSummary.PreUpdateOrganismLocations.Values).ToList();
            foreach (var pheromoneChangedLocation in pheromoneChangedLocations)
            {
                var x = pheromoneChangedLocation.X;
                var y = pheromoneChangedLocation.Y;
                this.EcosystemViewModel.HabitatViewModels[x][y].EnvironmentViewModel.Refresh();
            }

            // organism summary updates
            foreach (var organismViewModel in this.OrganismSummaryViewModel.OrganismViewModels)
            {
                organismViewModel.Refresh();
            }
        }
    }
}
