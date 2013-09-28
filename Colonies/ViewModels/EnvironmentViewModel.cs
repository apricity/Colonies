namespace Wacton.Colonies.ViewModels
{
    using System;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Ancillary;

    using Environment = Wacton.Colonies.Models.Environment;

    public class EnvironmentViewModel : ViewModelBase<Environment>
    {
        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
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

        public EnvironmentViewModel(Environment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            
        }

        public override void Refresh()
        {
            this.Terrain = this.DomainModel.Terrain;
            this.ObstructionLevel = this.DomainModel.GetLevel(Measure.Obstruction);
            this.PheromoneOpacity = this.DomainModel.GetLevel(Measure.Pheromone);
            this.NutrientScalar = this.DomainModel.GetLevel(Measure.Nutrient);
            this.MineralLevel = this.DomainModel.GetLevel(Measure.Mineral);
            this.DampLevel = this.DomainModel.GetLevel(Measure.Damp);
            this.HeatLevel = this.DomainModel.GetLevel(Measure.Heat);
        }
    }
}
