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

        public EcosystemViewModel(Ecosystem model, List<List<HabitatViewModel>> habitatViewModels, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            this.HabitatViewModels = habitatViewModels;
        }

        public void UpdateEcosystem(int turns)
        {
            for (var i = 0; i < turns; i++)
            {
                this.DomainModel.Update();
            }
        }
    }
}
