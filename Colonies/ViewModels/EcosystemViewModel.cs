namespace Colonies.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Threading;

    using Colonies.Annotations;
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

            //// TODO: make ecosystem model enumerable, with iteration of left-to-right, top-to-bottom (x then y)
            //this.HabitatViewModels = new List<List<HabitatViewModel>>();
            //for (var x = 0; x < this.EcosystemModel.Width; x++)
            //{
            //    this.habitatViewModels.Add(new List<HabitatViewModel>());
            //    for (var y = 0; y < this.EcosystemModel.Height; y++)
            //    {
            //        this.HabitatViewModels[x].Add(new HabitatViewModel(this.EcosystemModel.Habitats[x][y]));
            //    }
            //}
        }

        //public void StartEcosystem()
        //{
        //    this.ecosystemTimer = new Timer(this.Update, null, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
        //}

        //private void Update(object state)
        //{
        //    this.DomainModel.Update();
        //}

        //public void Update(object state)
        //{
        //    // currently, updating the ecosystem simply means
        //    // randomly moving all organisms to a different habitat
        //    var random = new Random();

        //    var occupiedHabitats = this.GetOccupiedHabitats();
        //    foreach (var occupiedHabitat in occupiedHabitats)
        //    {
        //        var destinationX = random.Next(this.EcosystemModel.Width);
        //        var destinationY = random.Next(this.EcosystemModel.Height);
        //        var destinationHabitat = this.HabitatViewModels[destinationX][destinationY];

        //        if (!destinationHabitat.Equals(occupiedHabitat))
        //        {
        //            this.MoveOrganism(occupiedHabitat, destinationHabitat);
        //        }
        //    }
        //}

        //private void MoveOrganism(HabitatViewModel sourceHabitat, HabitatViewModel destinationHabitat)
        //{
        //    if (!sourceHabitat.HabitatModel.ContainsOrganism())
        //    {
        //        throw new ArgumentException(String.Format("Source habitat {0} does not contain an organism", sourceHabitat), "sourceHabitat");
        //    }

        //    if (destinationHabitat.HabitatModel.ContainsOrganism())
        //    {
        //        throw new ArgumentException(String.Format("Desyination habitat {0} already contains an organism", destinationHabitat), "destinationHabitat");
        //    }

        //    var organismToMove = sourceHabitat.OrganismViewModel.OrganismModel;
        //    this.RemoveOrganism(sourceHabitat);
        //    this.AddOrganism(destinationHabitat, organismToMove);
        //}

        //private bool ContainsOrganism(int x, int y)
        //{
        //    return this.HabitatViewModels[x][y].HabitatModel.ContainsOrganism();
        //}

        //private void AddOrganism(HabitatViewModel habitat, Organism organism)
        //{
        //    if (habitat.HabitatModel.ContainsOrganism())
        //    {
        //        throw new ArgumentException(String.Format("Organism already exists at habitat {0}", habitat), "habitat");
        //    }

        //    habitat.OrganismViewModel.OrganismModel = organism;
        //}

        //private void RemoveOrganism(HabitatViewModel habitat)
        //{
        //    if (!habitat.HabitatModel.ContainsOrganism())
        //    {
        //        throw new ArgumentException(String.Format("No organism exists at habitat {0}", habitat), "habitat");
        //    }

        //    habitat.OrganismViewModel.OrganismModel = null;
        //}

        //private IEnumerable<HabitatViewModel> GetOccupiedHabitats()
        //{
        //    var occupiedHabitats = new List<HabitatViewModel>();
        //    foreach (var habitatList in this.HabitatViewModels)
        //    {
        //        foreach (var habitat in habitatList)
        //        {
        //            if (habitat.HabitatModel.ContainsOrganism())
        //            {
        //                occupiedHabitats.Add(habitat);
        //            }
        //        }
        //    }

        //    return occupiedHabitats;
        //}
    }
}
