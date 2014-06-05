namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;
    using Wacton.Colonies.ViewModels.Infrastructure;

    public class MainViewModel : ViewModelBase<IMain>
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

        public double MineralFormDemoninator
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

        public double ObstructionDemolishDenominator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.ObstructionDemolishRate;
            }
            set
            {
                this.DomainModel.Ecosystem.ObstructionDemolishRate = 1 / value;
                this.OnPropertyChanged("ObstructionDemolishRate");
            }
        }

        // TODO: needs add, spread, remove for damp, heat, poison...
        public double DampSpreadDenominator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.GetHazardChance(EnvironmentMeasure.Damp).SpreadChance;
            }
            set
            {
                var currentHazardChance = this.DomainModel.Ecosystem.GetHazardChance(EnvironmentMeasure.Damp);
                var updatedHazardChance = new HazardChance(currentHazardChance.AddChance, 1 / value, currentHazardChance.RemoveChance);
                this.DomainModel.Ecosystem.SetHazardChance(EnvironmentMeasure.Damp, updatedHazardChance);
                this.OnPropertyChanged("HazardSpreadChance");
            }
        }

        public static Color PheromoneColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Pheromone]; } }
        public static Color NutrientColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Nutrient]; } }
        public static Color MineralColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Mineral]; } }
        public static Color ObstructionColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Obstruction]; } }
        public static Color DampColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Damp]; } }
        public static Color HeatColor { get { return EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Heat]; } }

        private string weatherDampLevel;
        public string WeatherDampLevel
        {
            get
            {
                return this.weatherDampLevel;
            }
            set
            {
                this.weatherDampLevel = value;
                this.OnPropertyChanged("WeatherDampLevel");
            }
        }

        private string weatherHeatLevel;
        public string WeatherHeatLevel
        {
            get
            {
                return this.weatherHeatLevel;
            }
            set
            {
                this.weatherHeatLevel = value;
                this.OnPropertyChanged("WeatherHeatLevel");
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

        public MainViewModel(IMain domainModel, EcosystemViewModel ecosystemViewModel, OrganismSynopsisViewModel organismSynopsisViewModel, IEventAggregator eventAggregator)
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
                    this.WeatherDampLevel = string.Format("{0:0.0000}", this.DomainModel.Ecosystem.Weather.GetWeatherLevel(WeatherType.Damp));
                    this.WeatherHeatLevel = string.Format("{0:0.0000}", this.DomainModel.Ecosystem.Weather.GetWeatherLevel(WeatherType.Heat));

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
            var refreshedHabitatViewModels = new ConcurrentBag<HabitatViewModel>();

            // refresh properties of all altered environments
            Parallel.ForEach(updateSummary.AlteredEnvironmentCoordinates, alteredEnvironmentCoordinate =>
                {
                    var x = alteredEnvironmentCoordinate.X;
                    var y = alteredEnvironmentCoordinate.Y;

                    var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                    habitatViewModel.RefreshEnvironment();
                    refreshedHabitatViewModels.Add(habitatViewModel);
                });

            // refresh properties of all organisms that have not moved
            Parallel.ForEach(updateSummary.InactiveOrganismCoordinates, inactiveOrganismCoordinate =>
                {
                    var x = inactiveOrganismCoordinate.Value.X;
                    var y = inactiveOrganismCoordinate.Value.Y;

                    var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                    habitatViewModel.RefreshOrganism();
                    refreshedHabitatViewModels.Add(habitatViewModel);
                });

            // unassign moving organisms from their previous view models
            Parallel.ForEach(updateSummary.ActiveOrganismPreviousCoordinates, activeOrganismPreviousCoordinate =>
            {
                var x = activeOrganismPreviousCoordinate.Value.X;
                var y = activeOrganismPreviousCoordinate.Value.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                habitatViewModel.UnassignOrganismModel();
                refreshedHabitatViewModels.Add(habitatViewModel);
            });

            // assign moving organisms to their current view models
            Parallel.ForEach(updateSummary.ActiveOrganismCurrentCoordinates, activeOrganismCurrentCoordinate =>
            {
                var x = activeOrganismCurrentCoordinate.Value.X;
                var y = activeOrganismCurrentCoordinate.Value.Y;
                var organism = activeOrganismCurrentCoordinate.Key;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                habitatViewModel.AssignOrganismModel(organism);
                refreshedHabitatViewModels.Add(habitatViewModel);
            });

            // refresh the tool tip of each distinct habitat view model that has had its child view models refreshed
            Parallel.ForEach(refreshedHabitatViewModels.Distinct(), habitatViewModel => habitatViewModel.RefreshToolTip());

            this.EcosystemViewModel.RefreshWeatherColor();

            // refresh organism synopsis
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
