namespace Wacton.Colonies.ViewModels
{
    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

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

        private bool isObstructed;
        public bool IsObstructed
        {
            get
            {
                return this.isObstructed;
            }
            set
            {
                this.isObstructed = value;
                this.OnPropertyChanged("IsObstructed");
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

        private bool hasNutrient;
        public bool HasNutrient
        {
            get
            {
                return this.DomainModel.HasNutrient;
            }
            set
            {
                this.DomainModel.HasNutrient = value;
                this.OnPropertyChanged("HasNutrient");
            }
        }

        public EnvironmentViewModel(Environment domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            
        }

        public override void Refresh()
        {
            this.Terrain = this.DomainModel.Terrain;
            this.IsObstructed = this.DomainModel.IsObstructed;
            this.PheromoneOpacity = this.DomainModel.Pheromone.Level;
            this.HasNutrient = this.DomainModel.HasNutrient;
        }
    }
}
