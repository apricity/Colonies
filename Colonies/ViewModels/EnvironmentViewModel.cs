namespace Wacton.Colonies.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Logic;

    using Environment = Wacton.Colonies.Models.Environment;

    public class EnvironmentViewModel : ViewModelBase<Environment>
    {
        private readonly Color baseColor = Colors.Tan;
        private readonly Dictionary<Measure, Color> measureColors = new Dictionary<Measure, Color>
                                                                        {
                                                                            { Measure.Mineral, Colors.Goldenrod },
                                                                            { Measure.Damp, Colors.CornflowerBlue },
                                                                            { Measure.Heat, Colors.Tomato },
                                                                            { Measure.Poison, Colors.MediumAquamarine },
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
                return measureColors[Measure.Pheromone];
            }
        }

        public Color NutrientColor
        {
            get
            {
                return measureColors[Measure.Nutrient];
            }
        }

        public Color ObstructionColor
        {
            get
            {
                return measureColors[Measure.Obstruction];
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

        public EnvironmentViewModel(Environment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.CalculateEnvironmentColor();
        }

        public override void Refresh()
        {
            this.MineralLevel = this.DomainModel.GetLevel(Measure.Mineral);
            this.DampLevel = this.DomainModel.GetLevel(Measure.Damp);
            this.HeatLevel = this.DomainModel.GetLevel(Measure.Heat);
            this.PoisonLevel = this.DomainModel.GetLevel(Measure.Poison);

            this.PheromoneOpacity = this.DomainModel.GetLevel(Measure.Pheromone);
            this.NutrientScalar = this.DomainModel.GetLevel(Measure.Nutrient);
            this.ObstructionLevel = this.DomainModel.GetLevel(Measure.Obstruction);

            this.CalculateEnvironmentColor();
        }

        private void CalculateEnvironmentColor()
        {
            this.EnvironmentColor = ColorLogic.EnvironmentColor(
                this.baseColor,
                new WeightedColor(this.measureColors[Measure.Mineral], this.MineralLevel),
                new List<WeightedColor>
                    {
                        new WeightedColor(this.measureColors[Measure.Damp], this.DampLevel),
                        new WeightedColor(this.measureColors[Measure.Heat], this.HeatLevel),
                        new WeightedColor(this.measureColors[Measure.Poison], this.PoisonLevel)
                    });
        }
    }
}
