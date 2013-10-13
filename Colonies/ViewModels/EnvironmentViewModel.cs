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

        public SolidColorBrush MineralBrush { get; private set; }
        
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

        public SolidColorBrush DampBrush { get; private set; }

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

        public SolidColorBrush HeatBrush { get; private set; }

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

        public SolidColorBrush PoisonBrush { get; private set; }

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

        public SolidColorBrush PheromoneBrush { get; private set; }

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

        public SolidColorBrush NutrientBrush { get; private set; }

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

        public SolidColorBrush ObstructionBrush { get; private set; }

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
            this.MineralBrush = new SolidColorBrush(Colors.Goldenrod);
            this.DampBrush = new SolidColorBrush(Colors.CornflowerBlue);
            this.HeatBrush = new SolidColorBrush(Colors.Tomato);
            this.PoisonBrush = new SolidColorBrush(Colors.MediumAquamarine);
            this.PheromoneBrush = new SolidColorBrush(Colors.OrangeRed);
            this.NutrientBrush = new SolidColorBrush(Colors.OliveDrab);
            this.ObstructionBrush = new SolidColorBrush(Colors.Black);
        }

        public override void Refresh()
        {
            this.Terrain = this.DomainModel.Terrain;

            this.MineralLevel = this.DomainModel.GetLevel(Measure.Mineral);
            this.DampLevel = this.DomainModel.GetLevel(Measure.Damp);
            this.HeatLevel = this.DomainModel.GetLevel(Measure.Heat);
            this.PoisonLevel = this.DomainModel.GetLevel(Measure.Poison);
            this.PheromoneOpacity = this.DomainModel.GetLevel(Measure.Pheromone);
            this.NutrientScalar = this.DomainModel.GetLevel(Measure.Nutrient);
            this.ObstructionLevel = this.DomainModel.GetLevel(Measure.Obstruction);
        }
    }
}
