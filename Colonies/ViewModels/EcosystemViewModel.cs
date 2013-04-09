namespace Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Windows;

    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class EcosystemViewModel : ViewModelBase<Ecosystem>
    {
        private EnvironmentViewModel[,] environmentViewModels;
        public EnvironmentViewModel[,] EnvironmentViewModels
        {
            get
            {
                return this.environmentViewModels;
            }
            set
            {
                this.environmentViewModels = value;
                this.OnPropertyChanged("EnvironmentViewModels");
            }
        }

        private Dictionary<OrganismViewModel, Point> organismViewModels;
        public Dictionary<OrganismViewModel, Point> OrganismViewModels
        {
            get
            {
                return this.organismViewModels;
            }
            set
            {
                this.organismViewModels = value;
                this.OnPropertyChanged("OrganismViewModels");
            }
        }

        // TODO: generate view models from the above properties?
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

        public EcosystemViewModel(Ecosystem domainModel, EnvironmentViewModel[,] environmentViewModels, Dictionary<OrganismViewModel, Point> organismViewModels, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EnvironmentViewModels = environmentViewModels;
            this.OrganismViewModels = organismViewModels;

            var habitatViewModelGrid = new List<List<HabitatViewModel>>();
            for (int x = 0; x < this.EnvironmentViewModels.GetLength(0); x++)
            {
                var habitatViewModelColumn = new List<HabitatViewModel>();
                for (int y = 0; y < this.EnvironmentViewModels.GetLength(1); y++)
                {
                    var habitat = new Habitat(this.DomainModel.Environments[x, y], null);
                    habitatViewModelColumn.Add(new HabitatViewModel(habitat, this.EnvironmentViewModels[x, y], new OrganismViewModel(null, eventAggregator), eventAggregator));
                }

                habitatViewModelGrid.Add(habitatViewModelColumn);
            }

            this.HabitatViewModels = habitatViewModelGrid;
        }

        public void ProgressEcosystemOneTurn()
        {
            this.DomainModel.Update();
        }
    }
}
