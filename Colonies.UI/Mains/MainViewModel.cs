namespace Wacton.Colonies.UI.Mains
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Mains;
    using Wacton.Colonies.Domain.Weathers;
    using Wacton.Colonies.UI.Ecosystems;
    using Wacton.Colonies.UI.Habitats;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Colonies.UI.OrganismSynopses;
    using Wacton.Colonies.UI.Settings;

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

        private SettingsViewModel settingsViewModel;
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return this.settingsViewModel;
            }
            set
            {
                this.settingsViewModel = value;
                this.OnPropertyChanged(nameof(this.SettingsViewModel));
            }
        }

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

        private DateTime previousPhaseStartTime = DateTime.MinValue;
        private DateTime previousRoundStartTime = DateTime.MinValue;

        private int previousPhaseTimerInterval;

        public MainViewModel(IMain domainModel, SettingsViewModel settingsViewModel, EcosystemViewModel ecosystemViewModel, OrganismSynopsisViewModel organismSynopsisViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.SettingsViewModel = settingsViewModel;
            this.SettingsViewModel.OnToggleEcosystemActive(this.ChangeEcosystemPhaseTimer);
            this.EcosystemViewModel = ecosystemViewModel;
            this.OrganismSynopsisViewModel = organismSynopsisViewModel;

            this.RoundCount = 0;
            this.PhaseCount = 0;
            this.ecosystemPhaseTimer = new Timer(this.PerformEcosystemPhase);
            this.PhaseDuration = 0;
            this.RoundDuration = 0;

            // hook up a toggle ecosystem command so a keyboard shortcut can be used to toggle the ecosystem on/off
            this.ToggleEcosystemActiveCommand = new RelayCommand(o => this.SettingsViewModel.ToggleEcosystemActive());
            this.IncreasePhaseTimerIntervalCommand = new RelayCommand(o => this.SettingsViewModel.IncreasePhaseTimerInterval());
            this.DecreasePhaseTimerIntervalCommand = new RelayCommand(o => this.SettingsViewModel.DecreasePhaseTimerInterval());
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
                    if (this.SettingsViewModel.PhaseTimerInterval != this.previousPhaseTimerInterval)
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

        private void ChangeEcosystemPhaseTimer()
        {
            const int ImmediateStart = 0;
            const int PreventStart = Timeout.Infinite;

            this.ecosystemPhaseTimer.Change(
                this.SettingsViewModel.IsEcosystemActive ? this.SettingsViewModel.PhaseTimerInterval : PreventStart,
                this.SettingsViewModel.PhaseTimerInterval);

            this.previousPhaseTimerInterval = this.SettingsViewModel.PhaseTimerInterval;
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
