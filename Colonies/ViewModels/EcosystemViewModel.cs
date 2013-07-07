namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public class EcosystemViewModel : ViewModelBase<Ecosystem>
    {
        private List<List<HabitatViewModel>> habitatViewModels;
        public List<List<HabitatViewModel>> HabitatViewModels
        {
            get
            {
                return this.habitatViewModels;
            }
            set
            {
                this.habitatViewModels = value;
                this.OnPropertyChanged("HabitatViewModels");
            }
        }

        public EcosystemViewModel(Ecosystem domainModel, List<List<HabitatViewModel>> habitatViewModels, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.HabitatViewModels = habitatViewModels;
        }

        public void ProgressEcosystemOneTurn()
        {
            var updateSummary = this.DomainModel.Update();

            // TODO: have a more 'standard' usage of the update summary, whatever that might be
            foreach (var preUpdateOrganismLocation in updateSummary.PreUpdateOrganismLocations)
            {
                var x = preUpdateOrganismLocation.Value.X;
                var y = preUpdateOrganismLocation.Value.Y;
                this.HabitatViewModels[x][y].RefreshOrganismViewModel();
            }

            foreach (var postUpdateOrganismLocation in updateSummary.PostUpdateOrganismLocations)
            {
                var x = postUpdateOrganismLocation.Value.X;
                var y = postUpdateOrganismLocation.Value.Y;
                this.HabitatViewModels[x][y].RefreshOrganismViewModel();
            }

            var pheromoneChangedLocations =
                updateSummary.PheromoneDecreasedLocations.Union(
                    updateSummary.PreUpdateOrganismLocations.Values).ToList();
            foreach (var pheromoneChangedLocation in pheromoneChangedLocations)
            {
                var x = pheromoneChangedLocation.X;
                var y = pheromoneChangedLocation.Y;
                this.HabitatViewModels[x][y].RefreshEnvironmentViewModel();
            }
        }
    }
}
