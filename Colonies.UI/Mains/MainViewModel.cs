namespace Wacton.Colonies.UI.Mains
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Mains;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Weathers;
    using Wacton.Colonies.UI.Ecosystems;
    using Wacton.Colonies.UI.Environments;
    using Wacton.Colonies.UI.Habitats;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Colonies.UI.OrganismSynopses;
    using Wacton.Colonies.UI.Properties;

    // TODO: stop breaking the law of demeter so badly :(
    public class MainViewModel : ViewModelBase<IMain>
    {
        // if the timer interval is too small, the model update won't have finished
        // so use a lock to ensure the model isn't updated while it's updating...
        // (volatile because, if interval update is too small, lock will be accessed by multiple threads simultaneously)
        private readonly Timer ecosystemPhaseTimer;
        private volatile object performPhaseLock = new object();

        public ICommand ToggleEcosystemActiveCommand { get; set; }
        public ICommand IncreasePhaseTimerIntervalCommand { get; set; }
        public ICommand DecreasePhaseTimerIntervalCommand { get; set; }

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
                this.OnPropertyChanged(nameof(this.EcosystemViewModel));
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
                this.OnPropertyChanged(nameof(this.OrganismSynopsisViewModel));
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
                this.OnPropertyChanged(nameof(this.IsEcosystemActive));

                // if the ecosystem turns on/off the timer needs to start/stop 
                this.ChangeEcosystemPhaseTimer();
            }
        }

        // TODO: should the slider go from "slow (1) -> fast (100)", and that value be converted in this view model to a ms value?
        // phase interval is in ms
        private int phaseTimerInterval;
        public int PhaseTimerInterval
        {
            get
            {
                return this.phaseTimerInterval;
            }
            set
            {
                this.phaseTimerInterval = value;
                this.OnPropertyChanged(nameof(this.PhaseTimerInterval));
            }
        }

        private int previousPhaseTimerInterval;

        public double HealthDeteriorationDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[OrganismMeasure.Health];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[OrganismMeasure.Health] = 1 / value;
                this.OnPropertyChanged(nameof(this.HealthDeteriorationDemoninator));
            }
        }

        public double PheromoneDepositDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Pheromone];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Pheromone] = 1 / value;
                this.OnPropertyChanged(nameof(this.PheromoneDepositDemoninator));
            }
        }

        public double PheromoneFadeDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[EnvironmentMeasure.Pheromone];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[EnvironmentMeasure.Pheromone] = 1 / value;
                this.OnPropertyChanged(nameof(this.PheromoneFadeDemoninator));
            }
        }

        public double NutrientGrowthDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Nutrient];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Nutrient] = 1 / value;
                this.OnPropertyChanged(nameof(this.NutrientGrowthDemoninator));
            }
        }

        public double MineralFormDemoninator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Nutrient];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.IncreasingRates[EnvironmentMeasure.Nutrient] = 1 / value;
                this.OnPropertyChanged(nameof(this.MineralFormDemoninator));
            }
        }

        public double ObstructionDemolishDenominator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[EnvironmentMeasure.Obstruction];
            }
            set
            {
                this.DomainModel.Ecosystem.EcosystemRates.DecreasingRates[EnvironmentMeasure.Obstruction] = 1 / value;
                this.OnPropertyChanged(nameof(this.ObstructionDemolishDenominator));
            }
        }

        // TODO: needs add, spread, remove for damp, heat, disease...
        public double DampSpreadDenominator
        {
            get
            {
                return 1 / this.DomainModel.Ecosystem.EcosystemRates.HazardRates[EnvironmentMeasure.Damp].SpreadRate;
            }
            set
            {
                var currentHazardRate = this.DomainModel.Ecosystem.EcosystemRates.HazardRates[EnvironmentMeasure.Damp];
                var updatedHazardRate = new HazardRate(currentHazardRate.AddRate, 1 / value, currentHazardRate.RemoveRate);
                this.DomainModel.Ecosystem.EcosystemRates.HazardRates[EnvironmentMeasure.Damp] = updatedHazardRate;
                this.OnPropertyChanged(nameof(this.DampSpreadDenominator));
            }
        }

        public static Color PheromoneColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Pheromone];
        public static Color NutrientColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Nutrient];
        public static Color MineralColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Mineral];
        public static Color ObstructionColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Obstruction];
        public static Color SoundColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Sound];
        public static Color DampColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Damp];
        public static Color HeatColor => EnvironmentViewModel.MeasureColors[EnvironmentMeasure.Heat];

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
                this.OnPropertyChanged(nameof(this.WeatherDampLevel));
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
                this.OnPropertyChanged(nameof(this.WeatherHeatLevel));
            }
        }

        private int phaseCount;
        public int PhaseCount
        {
            get
            {
                return this.phaseCount;
            }
            private set
            {
                this.phaseCount = value;
                this.OnPropertyChanged(nameof(this.PhaseCount));
            }
        }

        private int phaseDuration;
        public int PhaseDuration
        {
            get
            {
                return this.phaseDuration;
            }
            private set
            {
                this.phaseDuration = value;
                this.OnPropertyChanged(nameof(this.PhaseDuration));
            }
        }

        private int roundCount;
        public int RoundCount
        {
            get
            {
                return this.roundCount;
            }
            private set
            {
                this.roundCount = value;
                this.OnPropertyChanged(nameof(this.RoundCount));
            }
        }

        private int roundDuration;
        public int RoundDuration
        {
            get
            {
                return this.roundDuration;
            }
            private set
            {
                this.roundDuration = value;
                this.OnPropertyChanged(nameof(this.RoundDuration));
            }
        }

        private DateTime previousPhaseStartTime = DateTime.MinValue;
        private DateTime previousRoundStartTime = DateTime.MinValue;

        public MainViewModel(IMain domainModel, EcosystemViewModel ecosystemViewModel, OrganismSynopsisViewModel organismSynopsisViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EcosystemViewModel = ecosystemViewModel;
            this.OrganismSynopsisViewModel = organismSynopsisViewModel;

            // initally set the ecosystem up to be not running
            this.RoundCount = 0;
            this.PhaseCount = 0;
            this.ecosystemPhaseTimer = new Timer(this.PerformEcosystemPhase);
            this.IsEcosystemActive = false;
            var initialPhaseTimerInterval = Settings.Default.PhaseTimerIntervalInMs;
            this.PhaseTimerInterval = initialPhaseTimerInterval;
            this.previousPhaseTimerInterval = initialPhaseTimerInterval;
            this.PhaseDuration = 0;
            this.RoundDuration = 0;

            // hook up a toggle ecosystem command so a keyboard shortcut can be used to toggle the ecosystem on/off
            this.ToggleEcosystemActiveCommand = new RelayCommand(this.ToggleEcosystemActive);
            this.IncreasePhaseTimerIntervalCommand = new RelayCommand(this.IncreasePhaseTimerInterval);
            this.DecreasePhaseTimerIntervalCommand = new RelayCommand(this.DecreasePhaseTimerInterval);
        }

        private void ToggleEcosystemActive(object obj)
        {
            this.IsEcosystemActive = !this.IsEcosystemActive;
        }

        // TODO: bind slider max and min to these values
        private void IncreasePhaseTimerInterval(object obj)
        {
            this.PhaseTimerInterval++;
            if (this.PhaseTimerInterval > 2000)
            {
                this.PhaseTimerInterval = 2000;
            }
        }

        private void DecreasePhaseTimerInterval(object obj)
        {
            this.PhaseTimerInterval--;
            if (this.PhaseTimerInterval < 1)
            {
                this.PhaseTimerInterval = 1;
            }
        }

        private void ChangeEcosystemPhaseTimer()
        {
            const int ImmediateStart = 0;
            const int PreventStart = Timeout.Infinite;

            this.ecosystemPhaseTimer.Change(this.IsEcosystemActive ? this.PhaseTimerInterval : PreventStart, this.PhaseTimerInterval);
            this.previousPhaseTimerInterval = this.PhaseTimerInterval;
        }

        private void PerformEcosystemPhase(object state)
        {
            if (Monitor.TryEnter(this.performPhaseLock))
            {
                try
                {
                    var phaseSummary = this.DomainModel.PerformPhase();
                    this.UpdateViewModels(phaseSummary);
                    this.PhaseCount = phaseSummary.PhaseNumber;

                    // TODO: only do these after all phases have been performed?
                    var previousRoundCount = this.RoundCount;
                    this.RoundCount = this.PhaseCount / phaseSummary.PhasesPerRound;
                    this.WeatherDampLevel = $"{this.DomainModel.Ecosystem.Weather.GetLevel(WeatherType.Damp):0.0000}";
                    this.WeatherHeatLevel = $"{this.DomainModel.Ecosystem.Weather.GetLevel(WeatherType.Heat):0.0000}";

                    // if there's been a change in the phase interval while the previous phase was processed
                    // update the interval of the ecosystem timer
                    if (this.PhaseTimerInterval != this.previousPhaseTimerInterval)
                    {
                        this.ChangeEcosystemPhaseTimer();
                    }

                    this.CalculateDuration(previousRoundCount);
                }
                finally
                {
                    Monitor.Exit(this.performPhaseLock);
                }
            }
        }

        private void CalculateDuration(int previousRoundCount)
        {
            if (this.previousPhaseStartTime.Equals(DateTime.MinValue))
            {
                this.previousPhaseStartTime = DateTime.Now;
            }
            else
            {
                var phaseStartTime = DateTime.Now;
                this.PhaseDuration = (int)(phaseStartTime - this.previousPhaseStartTime).TotalMilliseconds;
                this.previousPhaseStartTime = phaseStartTime;
            }

            if (this.previousRoundStartTime.Equals(DateTime.MinValue))
            {
                this.previousRoundStartTime = DateTime.Now;
            }
            else
            {
                if (this.RoundCount > previousRoundCount)
                {
                    var roundStartTime = DateTime.Now;
                    this.RoundDuration = (int)(roundStartTime - this.previousRoundStartTime).TotalMilliseconds;
                    this.previousRoundStartTime = roundStartTime;
                }
            }
        }

        private void UpdateViewModels(PhaseSummary phaseSummary)
        {
            var updatedHabitatViewModels = new ConcurrentBag<HabitatViewModel>();

            // update properties of all modifications
            Parallel.ForEach(phaseSummary.EcosystemHistory.Modifications, modification =>
            {
                var x = modification.Coordinate.X;
                var y = modification.Coordinate.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                updatedHabitatViewModels.Add(habitatViewModel);
            });

            // update properties of all organisms that have been added
            Parallel.ForEach(phaseSummary.EcosystemHistory.Additions, addition =>
            {
                var x = addition.Coordinate.X;
                var y = addition.Coordinate.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                habitatViewModel.AssignOrganismModel(addition.Organism);
                updatedHabitatViewModels.Add(habitatViewModel);

                this.OrganismSynopsisViewModel.AddOrganism(addition.Organism);
            });

            // update properties of all organisms that have not moved
            Parallel.ForEach(phaseSummary.OrganismCoordinates, organismCoordinate =>
            {
                var organism = organismCoordinate.Key;

                if (phaseSummary.EcosystemHistory.Relocations.Any(relocation => relocation.Organism.Equals(organism)))
                {
                    return;
                }

                var x = organismCoordinate.Value.X;
                var y = organismCoordinate.Value.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                updatedHabitatViewModels.Add(habitatViewModel);
            });

            // unassign moving organisms from their previous view models
            Parallel.ForEach(phaseSummary.EcosystemHistory.Relocations, relocation =>
            {
                var x = relocation.PreviousCoordinate.X;
                var y = relocation.PreviousCoordinate.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                habitatViewModel.UnassignOrganismModel();
                updatedHabitatViewModels.Add(habitatViewModel);
            });

            // assign moving organisms to their current view models
            Parallel.ForEach(phaseSummary.EcosystemHistory.Relocations, relocation =>
            {
                var x = relocation.UpdatedCoordinate.X;
                var y = relocation.UpdatedCoordinate.Y;

                var habitatViewModel = this.EcosystemViewModel.HabitatViewModels[x][y];
                habitatViewModel.AssignOrganismModel(relocation.Organism);
                updatedHabitatViewModels.Add(habitatViewModel);
            });

            // refresh each distinct habitat view model that has been updated
            Parallel.ForEach(updatedHabitatViewModels.Distinct(), habitatViewModel => habitatViewModel.Refresh());

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
