namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;
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

        private OrganismSynopsisViewModel organismSynopsisViewModel;
        public OrganismSynopsisViewModel OrganismSynopsisViewModel
        {
            get
            {
                return this.organismSynopsisViewModel;
            }
            set
            {
                this.organismSynopsisViewModel = value;
                this.OnPropertyChanged("OrganismSynopsisViewModel");
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

        public double HealthDeteriorationDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.HealthDeteriorationRate;
            }
            set
            {
                this.DomainModel.Ecosystem.HealthDeteriorationRate = 1 / value;
                this.OnPropertyChanged("HealthDeteriorationDemoninator");
            }
        }

        public double PheromoneDepositDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.PheromoneDepositRate;
            }
            set
            {
                this.DomainModel.Ecosystem.PheromoneDepositRate = 1 / value;
                this.OnPropertyChanged("PheromoneDepositDemoninator");
            }
        }

        public double PheromoneFadeDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.PheromoneFadeRate;
            }
            set
            {
                this.DomainModel.Ecosystem.PheromoneFadeRate = 1 / value;
                this.OnPropertyChanged("PheromoneFadeDemoninator");
            }
        }

        public double NutrientGrowthDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.NutrientGrowthRate;
            }
            set
            {
                this.DomainModel.Ecosystem.NutrientGrowthRate = 1 / value;
                this.OnPropertyChanged("NutrientGrowthRate");
            }
        }

        public double MineralGrowthDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.MineralFormRate;
            }
            set
            {
                this.DomainModel.Ecosystem.MineralFormRate = 1 / value;
                this.OnPropertyChanged("MineralFormRate");
            }
        }

        private int turnCount;
        public int TurnCount
        {
            get
            {
                return this.turnCount;
            }
            private set
            {
                this.turnCount = value;
                this.OnPropertyChanged("TurnCount");
            }
        }

        public MainViewModel(Main domainModel, EcosystemViewModel ecosystemViewModel, OrganismSynopsisViewModel organismSynopsisViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
            this.OrganismSynopsisViewModel = organismSynopsisViewModel;

            // initally set the ecosystem up to be not running
            this.TurnCount = 0;
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
            if (Monitor.TryEnter(this.updateLock))
            {
                try
                {
                    var updateSummary = this.DomainModel.UpdateOnce();
                    this.UpdateViewModels(updateSummary);
                    this.TurnCount++;

                    // if there's been a change in the turn interval while the previous turn was processed
                    // update the interval of the ecosystem timer
                    if (this.EcosystemTurnInterval != this.lastUsedTurnInterval)
                    {
                        this.ChangeEcosystemTimer();
                    }
                }
                finally
                {
                    Monitor.Exit(this.updateLock);
                }
            }
        }

        private void UpdateViewModels(UpdateSummary updateSummary)
        {
            // ecosystem updates
            var environmentCoordinatesToUpdate = new List<Coordinate>();
            environmentCoordinatesToUpdate.AddRange(updateSummary.PheromoneDecreasedLocations);
            environmentCoordinatesToUpdate.AddRange(updateSummary.PreUpdateOrganismLocations); // where pheromone has been deposited and mineral formed
            environmentCoordinatesToUpdate.AddRange(updateSummary.NutrientGrowthLocations);
            environmentCoordinatesToUpdate.AddRange(updateSummary.ObstructionDemolishLocations);
            environmentCoordinatesToUpdate = environmentCoordinatesToUpdate.Distinct().ToList();

            Parallel.ForEach(environmentCoordinatesToUpdate, location =>
                {
                    var x = location.X;
                    var y = location.Y;
                    this.EcosystemViewModel.HabitatViewModels[x][y].RefreshEnvironment();
                });

            foreach (var preUpdateOrganismLocation in updateSummary.PreUpdateOrganismLocations)
            {
                var x = preUpdateOrganismLocation.X;
                var y = preUpdateOrganismLocation.Y;
                this.EcosystemViewModel.HabitatViewModels[x][y].RemoveOrganismModel();
            }

            foreach (var postUpdateOrganismLocation in updateSummary.PostUpdateOrganismLocations)
            {
                var x = postUpdateOrganismLocation.X;
                var y = postUpdateOrganismLocation.Y;
                var organism = this.DomainModel.Ecosystem.Habitats[x, y].Organism;
                this.EcosystemViewModel.HabitatViewModels[x][y].AssignOrganismModel(organism);
            }

            // organism synopsis updates
            this.OrganismSynopsisViewModel.Refresh();
        }

        public override void Refresh()
        {
            // refresh all child view models (ecosystem and organism synopsis)
            this.EcosystemViewModel.Refresh();
            this.OrganismSynopsisViewModel.Refresh();
        }
    }
}
