namespace Wacton.Colonies.UI.Environments
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Environments;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Tovarisch.Color;

    public class EnvironmentViewModel : ViewModelBase<IEnvironment>
    {
        private readonly Color baseColor = Colors.DimGray;

        public static readonly Dictionary<EnvironmentMeasure, Color> MeasureColors =
            new Dictionary<EnvironmentMeasure, Color>
                {
                    { EnvironmentMeasure.Mineral, Colors.Goldenrod },
                    { EnvironmentMeasure.Damp, Colors.CornflowerBlue },
                    { EnvironmentMeasure.Heat, Colors.Tomato },
                    { EnvironmentMeasure.Disease, Colors.YellowGreen },
                    { EnvironmentMeasure.Pheromone, Colors.OrangeRed },
                    { EnvironmentMeasure.Nutrient, Colors.OliveDrab },
                    { EnvironmentMeasure.Obstruction, Colors.Black },
                    { EnvironmentMeasure.Sound, Color.FromArgb(100, 255, 255, 255) }
                };

        private Color environmentColor;
        public Color EnvironmentColor
        {
            get
            {
                return this.environmentColor;
            }
            set
            {
                this.environmentColor = value;
                this.OnPropertyChanged(nameof(this.EnvironmentColor));
            }
        }

        public Color PheromoneColor => MeasureColors[EnvironmentMeasure.Pheromone];
        public Color NutrientColor => MeasureColors[EnvironmentMeasure.Nutrient];
        public Color ObstructionColor => MeasureColors[EnvironmentMeasure.Obstruction];
        public Color SoundColor => MeasureColors[EnvironmentMeasure.Sound];

        private double mineralLevel;
        public double MineralLevel
        {
            get
            {
                return this.mineralLevel;
            }
            set
            {
                this.mineralLevel = value;
                this.OnPropertyChanged(nameof(this.MineralLevel));
            }
        }

        private double dampLevel;
        public double DampLevel
        {
            get
            {
                return this.dampLevel;
            }
            set
            {
                this.dampLevel = value;
                this.OnPropertyChanged(nameof(this.DampLevel));
            }
        }

        private double heatLevel;
        public double HeatLevel
        {
            get
            {
                return this.heatLevel;
            }
            set
            {
                this.heatLevel = value;
                this.OnPropertyChanged(nameof(this.HeatLevel));
            }
        }

        private double diseaseLevel;
        public double DiseaseLevel
        {
            get
            {
                return this.diseaseLevel;
            }
            set
            {
                this.diseaseLevel = value;
                this.OnPropertyChanged(nameof(this.DiseaseLevel));
            }
        }

        private double pheromoneOpacity;
        public double PheromoneOpacity
        {
            get
            {
                return this.pheromoneOpacity;
            }
            set
            {
                this.pheromoneOpacity = value;
                this.OnPropertyChanged(nameof(this.PheromoneOpacity));
            }
        }

        private double nutrientScalar;
        public double NutrientScalar
        {
            get
            {
                return this.nutrientScalar;
            }
            set
            {
                this.nutrientScalar = value;
                this.OnPropertyChanged(nameof(this.NutrientScalar));
            }
        }

        private double obstructionLevel;
        public double ObstructionLevel
        {
            get
            {
                return this.obstructionLevel;
            }
            set
            {
                this.obstructionLevel = value;
                this.OnPropertyChanged(nameof(this.ObstructionLevel));

                this.IsObstructed = Math.Abs(value - 0.0) > 0.0;
                this.OnPropertyChanged(nameof(this.IsObstructed));
            }
        }

        public bool IsObstructed { get; private set; }

        private double soundLevel;
        public double SoundLevel
        {
            get
            {
                return this.soundLevel;
            }
            set
            {
                this.soundLevel = value;
                this.OnPropertyChanged(nameof(this.SoundLevel));
            }
        }

        public EnvironmentViewModel(IEnvironment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
        }

        public override void Refresh()
        {
            this.MineralLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Mineral);
            this.DampLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Damp);
            this.HeatLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Heat);
            this.DiseaseLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Disease);

            this.PheromoneOpacity = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Pheromone);
            this.NutrientScalar = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Nutrient);
            this.ObstructionLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Obstruction);
            this.SoundLevel = this.DomainModel.MeasurementData.GetLevel(EnvironmentMeasure.Sound);

            this.RefreshEnvironmentColor();
        }

        private void RefreshEnvironmentColor()
        {
            var mineralWeightedColor = new WeightedColor(MeasureColors[EnvironmentMeasure.Mineral], this.MineralLevel);
            var hazardWeightedColors = new List<WeightedColor>
                    {
                        new WeightedColor(MeasureColors[EnvironmentMeasure.Damp], this.DampLevel),
                        new WeightedColor(MeasureColors[EnvironmentMeasure.Heat], this.HeatLevel),
                        new WeightedColor(MeasureColors[EnvironmentMeasure.Disease], this.DiseaseLevel)
                    };

            var preHazardColor = ColorLogic.ModifyColor(this.baseColor, new List<WeightedColor> { mineralWeightedColor });
            this.EnvironmentColor = ColorLogic.ModifyColor(preHazardColor, hazardWeightedColors);
        }
    }
}
