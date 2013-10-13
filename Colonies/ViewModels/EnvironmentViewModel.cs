namespace Wacton.Colonies.ViewModels
{
    using System;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;

    using Environment = Wacton.Colonies.Models.Environment;

    public class EnvironmentViewModel : ViewModelBase<Environment>
    {
        public Color BaseColor { get; private set; }

        private Terrain terrain;
        public Terrain Terrain
        {
            get
            {
                return this.terrain;
            }
            set
            {
                this.terrain = value;
                this.OnPropertyChanged("Terrain");
            }
        }

        private WeightedColor mineralLevel;
        public WeightedColor MineralLevel
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

        private WeightedColor dampLevel;
        public WeightedColor DampLevel
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

        private WeightedColor heatLevel;
        public WeightedColor HeatLevel
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

        private WeightedColor poisonLevel;
        public WeightedColor PoisonLevel
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
            this.BaseColor = Colors.Tan;
        }

        public override void Refresh()
        {
            this.Terrain = this.DomainModel.Terrain;

            this.MineralLevel = new WeightedColor(Colors.Goldenrod, this.DomainModel.GetLevel(Measure.Mineral));
            this.DampLevel = new WeightedColor(Colors.CornflowerBlue, this.DomainModel.GetLevel(Measure.Damp));
            this.HeatLevel = new WeightedColor(Colors.Tomato, this.DomainModel.GetLevel(Measure.Heat));
            this.PoisonLevel = new WeightedColor(Colors.MediumAquamarine, this.DomainModel.GetLevel(Measure.Poison));

            this.NutrientScalar = this.DomainModel.GetLevel(Measure.Nutrient);
            this.PheromoneOpacity = this.DomainModel.GetLevel(Measure.Pheromone);
            this.ObstructionLevel = this.DomainModel.GetLevel(Measure.Obstruction);
        }
    }
}
