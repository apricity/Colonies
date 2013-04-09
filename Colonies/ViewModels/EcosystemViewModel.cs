namespace Colonies.ViewModels
{
    using System.Collections.Generic;

    using Colonies.Events;
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
            // TODO: Update() to return a list of changes?  Then we don't have to be using message passing yet and can update ONLY the necessary habitats?
            // TODO: like this: this.HabitatViewModels[0][0].RefreshOrganismViewModel();
            this.DomainModel.Update();
            
            // spread the word that some organisms (might) have moved
            this.EventAggregator.GetEvent<OrganismsUpdatedEvent>().Publish(null);
        }
    }
}
