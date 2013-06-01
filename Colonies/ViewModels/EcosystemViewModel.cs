﻿namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class EcosystemViewModel : ViewModelBase<Ecosystem>
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

            // TODO: have a more 'standard' usage of the update summary
            foreach (var summary in updateSummary.PreUpdateSummary)
            {
                var x = summary.Value.X;
                var y = summary.Value.Y;

                // TODO: not all previous environments will need updated
                // TODO: as some organisms might not be depositing pheromones
                // TODO: although... this will only update environments that an organism was on..?  need to update any that have decreased pheromone level... (quite a lot)
                this.HabitatViewModels[x][y].RefreshEnvironmentViewModel();
                this.HabitatViewModels[x][y].RefreshOrganismViewModel();
            }

            foreach (var summary in updateSummary.PostUpdateSummary)
            {
                var x = summary.Value.X;
                var y = summary.Value.Y;

                this.HabitatViewModels[x][y].RefreshOrganismViewModel();
            }
        }
    }
}
