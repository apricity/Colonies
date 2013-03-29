namespace Colonies.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Threading;

    using Colonies.Annotations;
    using Colonies.Models;

    public sealed class EcosystemViewModel : INotifyPropertyChanged
    {
        private Timer ecosystemTimer;

        private Ecosystem ecosystemModel;
        public Ecosystem EcosystemModel
        {
            get
            {
                return this.ecosystemModel;
            }
            set
            {
                this.ecosystemModel = value;
                this.OnPropertyChanged("EcosystemModel");
            }
        }

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

        public EcosystemViewModel(Ecosystem model)
        {
            this.EcosystemModel = model;

            this.HabitatViewModels = new List<List<HabitatViewModel>>();
            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                this.habitatViewModels.Add(new List<HabitatViewModel>());
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    this.HabitatViewModels[x].Add(new HabitatViewModel(this.EcosystemModel.Habitats[x][y]));
                }
            }
        }

        public void StartEcosystem()
        {
            TimerCallback modelTimerCallback = this.UpdateModel;
            this.ecosystemTimer = new Timer(modelTimerCallback, null, 2000, 1000);
        }

        private void UpdateModel(object state)
        {
            var organismViewModels = this.GetOrganismViewmodels();
            foreach (var organismViewModel in organismViewModels)
            {
                this.MoveOrganismToRandomAvailableHabitat(organismViewModel.OrganismModel);
            }
        }

        private void AddOrganism(int x, int y, Organism organism)
        {
            if (this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("Organism already occupies coordinates ({0}, {1})", x, y));
            }

            this.HabitatViewModels[x][y].OrganismViewModel.OrganismModel = organism;
        }

        private void RemoveOrganism(int x, int y)
        {
            if (!this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("No organism exists at coordinates ({0}, {1})", x, y));
            }

            this.HabitatViewModels[x][y].OrganismViewModel.OrganismModel = null;
        }

        private IEnumerable<OrganismViewModel> GetOrganismViewmodels()
        {
            var organisms = new List<OrganismViewModel>();

            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        organisms.Add(this.HabitatViewModels[x][y].OrganismViewModel);
                    }
                }
            }

            return organisms;
        }

        private bool ContainsOrganism(int x, int y)
        {
            return this.HabitatViewModels[x][y].OrganismViewModel.OrganismModel != null;
        }

        private HabitatViewModel GetHabitatOfOrganism(Organism organism)
        {
            var habitatsOfOrganism = new List<HabitatViewModel>();

            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    if (this.ContainsOrganism(x, y) && this.HabitatViewModels[x][y].OrganismViewModel.OrganismModel.Equals(organism))
                    {
                        habitatsOfOrganism.Add(this.HabitatViewModels[x][y]);
                    }
                }
            }

            return habitatsOfOrganism.Single();
        }

        private void MoveOrganismToRandomAvailableHabitat(Organism organism)
        {
            var random = new Random();
            var targetColumn = random.Next(5);
            var targetRow = random.Next(5);

            if (!this.ContainsOrganism(targetColumn, targetRow))
            {
                var currentOrganismHabitat = this.GetHabitatOfOrganism(organism);
                currentOrganismHabitat.OrganismViewModel.OrganismModel = null;

                this.AddOrganism(targetColumn, targetRow, organism);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
