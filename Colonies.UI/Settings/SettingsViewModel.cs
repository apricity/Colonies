namespace Wacton.Colonies.UI.Settings
{
    using System;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Settings;
    using Wacton.Colonies.UI.Environments;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Colonies.UI.Properties;

    public class SettingsViewModel : ViewModelBase<IEcosystemSettings>
    {
        private Action onToggleEcosystemActiveAction;

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

                this.onToggleEcosystemActiveAction?.Invoke();
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

        public double HealthDeteriorationDemoninator
        {
            get
            {
                return 1 / this.DomainModel.DecreasingRates[OrganismMeasure.Health];
            }
            set
            {
                this.DomainModel.DecreasingRates[OrganismMeasure.Health] = 1 / value;
                this.OnPropertyChanged(nameof(this.HealthDeteriorationDemoninator));
            }
        }

        public double PheromoneDepositDemoninator
        {
            get
            {
                return 1 / this.DomainModel.IncreasingRates[EnvironmentMeasure.Pheromone];
            }
            set
            {
                this.DomainModel.IncreasingRates[EnvironmentMeasure.Pheromone] = 1 / value;
                this.OnPropertyChanged(nameof(this.PheromoneDepositDemoninator));
            }
        }

        public double PheromoneFadeDemoninator
        {
            get
            {
                return 1 / this.DomainModel.DecreasingRates[EnvironmentMeasure.Pheromone];
            }
            set
            {
                this.DomainModel.DecreasingRates[EnvironmentMeasure.Pheromone] = 1 / value;
                this.OnPropertyChanged(nameof(this.PheromoneFadeDemoninator));
            }
        }

        public double NutrientGrowthDemoninator
        {
            get
            {
                return 1 / this.DomainModel.IncreasingRates[EnvironmentMeasure.Nutrient];
            }
            set
            {
                this.DomainModel.IncreasingRates[EnvironmentMeasure.Nutrient] = 1 / value;
                this.OnPropertyChanged(nameof(this.NutrientGrowthDemoninator));
            }
        }

        public double MineralFormDemoninator
        {
            get
            {
                return 1 / this.DomainModel.IncreasingRates[EnvironmentMeasure.Nutrient];
            }
            set
            {
                this.DomainModel.IncreasingRates[EnvironmentMeasure.Nutrient] = 1 / value;
                this.OnPropertyChanged(nameof(this.MineralFormDemoninator));
            }
        }

        public double ObstructionDemolishDenominator
        {
            get
            {
                return 1 / this.DomainModel.DecreasingRates[EnvironmentMeasure.Obstruction];
            }
            set
            {
                this.DomainModel.DecreasingRates[EnvironmentMeasure.Obstruction] = 1 / value;
                this.OnPropertyChanged(nameof(this.ObstructionDemolishDenominator));
            }
        }

        // TODO: needs add, spread, remove for damp, heat, disease...
        public double DampSpreadDenominator
        {
            get
            {
                return 1 / this.DomainModel.HazardRates[EnvironmentMeasure.Damp].SpreadRate;
            }
            set
            {
                var currentHazardRate = this.DomainModel.HazardRates[EnvironmentMeasure.Damp];
                var updatedHazardRate = new HazardRate(currentHazardRate.AddRate, 1 / value, currentHazardRate.RemoveRate);
                this.DomainModel.HazardRates[EnvironmentMeasure.Damp] = updatedHazardRate;
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

        public SettingsViewModel(IEcosystemSettings domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            // initally set the ecosystem up to be not running
            this.IsEcosystemActive = false;
            this.PhaseTimerInterval = Settings.Default.PhaseTimerIntervalInMs;
        }

        public void OnToggleEcosystemActive(Action action)
        {
            this.onToggleEcosystemActiveAction = action;
        }

        public void ToggleEcosystemActive()
        {
            this.IsEcosystemActive = !this.IsEcosystemActive;
            this.onToggleEcosystemActiveAction();
        }

        // TODO: bind slider max and min to these values?
        public void IncreasePhaseTimerInterval()
        {
            this.PhaseTimerInterval++;
            if (this.PhaseTimerInterval > 2000)
            {
                this.PhaseTimerInterval = 2000;
            }
        }

        public void DecreasePhaseTimerInterval()
        {
            this.PhaseTimerInterval--;
            if (this.PhaseTimerInterval < 1)
            {
                this.PhaseTimerInterval = 1;
            }
        }

        public override void Refresh()
        {
            throw new System.NotImplementedException();
        }
    }
}
