namespace Colonies.ViewModels
{
    using System.Collections.Generic;

    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

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
