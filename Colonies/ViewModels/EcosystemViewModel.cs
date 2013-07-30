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

        public override void Refresh()
        {
            // refresh all child view models (each habitat)
            foreach (var habitatViewModel in this.HabitatViewModels.SelectMany(habitatViewModel => habitatViewModel))
            {
                habitatViewModel.Refresh();
            }
        }
    }
}
