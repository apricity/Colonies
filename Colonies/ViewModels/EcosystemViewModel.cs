using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;
    using System.Threading;

    using Colonies.Annotations;

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

        private List<List<NicheViewModel>> nicheViewModels;
        public List<List<NicheViewModel>> NicheViewModels
        {
            get
            {
                return this.nicheViewModels;
            }
            set
            {
                this.nicheViewModels = value;
                this.OnPropertyChanged("NicheViewModels");
            }
        }

        public EcosystemViewModel(Ecosystem model)
        {
            this.EcosystemModel = model;

            this.NicheViewModels = new List<List<NicheViewModel>>();
            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                nicheViewModels.Add(new List<NicheViewModel>());
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    this.NicheViewModels[x].Add(new NicheViewModel(this.EcosystemModel.Niches[x][y]));
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
            var organismViewModels = this.GetOrganisms();
            foreach (var organismViewModel in organismViewModels)
            {
                this.MoveOrganismToRandomAvailableNiche(organismViewModel.OrganismModel);
            }
        }

        public void AddOrganism(int x, int y, Organism organism)
        {
            if (this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("Organism already occupies coordinates ({0}, {1})", x, y));
            }

            this.NicheViewModels[x][y].OrganismViewModel.OrganismModel = organism;
        }

        public void RemoveOrganism(int x, int y)
        {
            if (!this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("No organism exists at coordinates ({0}, {1})", x, y));
            }

            this.NicheViewModels[x][y].OrganismViewModel.OrganismModel = null;
        }

        public List<OrganismViewModel> GetOrganisms()
        {
            var organisms = new List<OrganismViewModel>();

            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        organisms.Add(this.NicheViewModels[x][y].OrganismViewModel);
                    }
                }
            }

            return organisms;
        }

        public bool ContainsOrganism(int x, int y)
        {
            return this.NicheViewModels[x][y].OrganismViewModel.OrganismModel != null;
        }

        public NicheViewModel GetNiche(int x, int y)
        {
            return this.NicheViewModels[x][y];
        }

        public void SetNiche(int x, int y, Niche niche)
        {
            this.NicheViewModels[x][y].NicheModel = niche;
        }

        public List<NicheViewModel> GetOccupiedNiches()
        {
            var occupiedNiches = new List<NicheViewModel>();

            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        occupiedNiches.Add(this.GetNiche(x, y));
                    }
                }
            }

            return occupiedNiches;
        }

        private NicheViewModel GetNicheOfOrganism(Organism organism)
        {
            var nichesOfOrganism = new List<NicheViewModel>();

            for (int x = 0; x < this.EcosystemModel.Width; x++)
            {
                for (int y = 0; y < this.EcosystemModel.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        nichesOfOrganism.Add(this.NicheViewModels[x][y]);
                    }
                }
            }

            return nichesOfOrganism.Single();
        }

        public void MoveOrganismToRandomAvailableNiche(Organism organism)
        {
            var random = new Random();
            var targetColumn = random.Next(5);
            var targetRow = random.Next(5);

            if (!this.ContainsOrganism(targetColumn, targetRow))
            {
                var currentOrganismNiche = this.GetNicheOfOrganism(organism);
                currentOrganismNiche.OrganismViewModel.OrganismModel = null;

                this.AddOrganism(targetColumn, targetRow, organism);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
