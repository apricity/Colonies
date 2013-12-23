namespace Wacton.Colonies.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Logic;

    public class EnvironmentViewModel : ViewModelBase<IEnvironment>
    {
        private readonly Color baseColor = Colors.Tan;
        public static readonly Dictionary<Measure, Color> MeasureColors = new Dictionary<Measure, Color>
                                                                        {
                                                                            { Measure.Mineral, Colors.Goldenrod },
                                                                            { Measure.Damp, Colors.CornflowerBlue },
                                                                            { Measure.Heat, Colors.Tomato },
                                                                            { Measure.Poison, Colors.DimGray },
                                                                            { Measure.Pheromone, Colors.OrangeRed },
                                                                            { Measure.Nutrient, Colors.OliveDrab },
                                                                            { Measure.Obstruction, Colors.Black },
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
                this.OnPropertyChanged("EnvironmentColor");
            }
        }

        public Color PheromoneColor
        {
            get
            {
                return MeasureColors[Measure.Pheromone];
            }
        }

        public Color NutrientColor
        {
            get
            {
                return MeasureColors[Measure.Nutrient];
            }
        }

        public Color ObstructionColor
        {
            get
            {
                return MeasureColors[Measure.Obstruction];
            }
        }

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
                this.OnPropertyChanged("MineralLevel");
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
                this.OnPropertyChanged("DampLevel");
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
                this.OnPropertyChanged("HeatLevel");
            }
        }

        private double poisonLevel;
        public double PoisonLevel
        {
            get
            {
                return this.poisonLevel;
            }
            set
            {
                this.poisonLevel = value;
                this.OnPropertyChanged("PoisonLevel");
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
                this.OnPropertyChanged("PheromoneOpacity");
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
                this.OnPropertyChanged("NutrientScalar");
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
                this.OnPropertyChanged("ObstructionLevel");

                this.IsObstructed = Math.Abs(value - 0.0) > 0.0;
                this.OnPropertyChanged("IsObstructed");
            }
        }

        public bool IsObstructed { get; private set; }

        public EnvironmentViewModel(IEnvironment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.RefreshEnvironmentColor();
        }

        public override void Refresh()
        {
            this.MineralLevel = this.DomainModel.Measurement.GetLevel(Measure.Mineral);
            this.DampLevel = this.DomainModel.Measurement.GetLevel(Measure.Damp);
            this.HeatLevel = this.DomainModel.Measurement.GetLevel(Measure.Heat);
            this.PoisonLevel = this.DomainModel.Measurement.GetLevel(Measure.Poison);

            this.PheromoneOpacity = this.DomainModel.Measurement.GetLevel(Measure.Pheromone);
            this.NutrientScalar = this.DomainModel.Measurement.GetLevel(Measure.Nutrient);
            this.ObstructionLevel = this.DomainModel.Measurement.GetLevel(Measure.Obstruction);

            this.RefreshEnvironmentColor();
        }

        private void RefreshEnvironmentColor()
        {
            this.EnvironmentColor = ColorLogic.EnvironmentColor(
                this.baseColor,
                new WeightedColor(MeasureColors[Measure.Mineral], this.MineralLevel),
                new List<WeightedColor>
                    {
                        new WeightedColor(MeasureColors[Measure.Damp], this.DampLevel),
                        new WeightedColor(MeasureColors[Measure.Heat], this.HeatLevel),
                        new WeightedColor(MeasureColors[Measure.Poison], this.PoisonLevel)
                    });
        }
    }
}
