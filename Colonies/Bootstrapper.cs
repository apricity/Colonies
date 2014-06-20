namespace Wacton.Colonies
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models;
    using Wacton.Colonies.Models.DataProviders;
    using Wacton.Colonies.Models.Interfaces;
    using Wacton.Colonies.Properties;
    using Wacton.Colonies.ViewModels;
    using Wacton.Colonies.Views;

    public class Bootstrapper
    {
        public virtual void Run()
        {
            // get the version number to display on the main window title
            var assembly = Assembly.GetExecutingAssembly();
            var version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();

            // create the view to display to the user
            // the data context is the view model tree that contains the model
            var mainViewModel = this.BuildMainDataContext(Settings.Default.EcosystemWidth, Settings.Default.EcosystemHeight);
            mainViewModel.Refresh();

            var mainView = new MainView { DataContext = mainViewModel };
            mainView.Title += string.Format(" ({0})", version);

            // display the window to the user!
            mainView.Show();
        }

        protected MainViewModel BuildMainDataContext(int ecosystemWidth, int ecosystemHeight)
        {
            // the event aggregator might be used by view models to inform of changes
            var eventaggregator = new EventAggregator();

            var habitats = new Habitat[ecosystemWidth, ecosystemHeight];
            var habitatViewModels = new List<List<HabitatViewModel>>();

            for (var x = 0; x < ecosystemWidth; x++)
            {
                habitatViewModels.Add(new List<HabitatViewModel>());

                for (var y = 0; y < ecosystemHeight; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment();
                    var environmentViewModel = new EnvironmentViewModel(environment, eventaggregator);

                    var organismViewModel = new OrganismViewModel(null, eventaggregator);

                    var habitat = new Habitat(environment, null);
                    var habitatViewModel = new HabitatViewModel(habitat, environmentViewModel, organismViewModel, eventaggregator);

                    habitats[x, y] = habitat;
                    habitatViewModels[x].Add(habitatViewModel);
                }
            }

            var initialOrganismCoordinates = this.InitialOrganismCoordinates();
            var ecosystemData = new EcosystemData(habitats, initialOrganismCoordinates);
            var weather = new Weather();
            var environmentMeasureDistributor = new EnvironmentMeasureDistributor(ecosystemData);
            var ecosystem = new Ecosystem(ecosystemData, weather, environmentMeasureDistributor);
            var ecosystemViewModel = new EcosystemViewModel(ecosystem, habitatViewModels, eventaggregator);

            this.InitialiseTerrain(ecosystem);
            foreach (var organismCoordinate in ecosystemData.EmittingSoundOrganismCoordinates())
            {
                ecosystem.EnvironmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Sound);
            }

            // hook organism model into the ecosystem
            foreach (var organismLocation in initialOrganismCoordinates)
            {
                var organism = organismLocation.Key;
                var location = organismLocation.Value;

                habitatViewModels[location.X][location.Y].OrganismViewModel.AssignModel(organism);
            }

            // hook organism model into the organism synopsis
            var organismSynopsis = new OrganismSynopsis(initialOrganismCoordinates.Keys.Select(organism => (IOrganism)organism).ToList());
            var organismViewModels =
                organismSynopsis.Organisms.Select(organism => new OrganismViewModel(organism, eventaggregator)).ToList();
            var organismSynopsisViewModel = new OrganismSynopsisViewModel(organismSynopsis, organismViewModels, eventaggregator);

            var main = new Main(ecosystem);
            var mainViewModel = new MainViewModel(main, ecosystemViewModel, organismSynopsisViewModel, eventaggregator);

            return mainViewModel;
        }

        protected virtual void InitialiseTerrain(Ecosystem ecosystem)
        {
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(19, 0), EnvironmentMeasure.Damp);
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(15, 3), EnvironmentMeasure.Damp);
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(17, 4), EnvironmentMeasure.Damp);
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(17, 5), EnvironmentMeasure.Heat);
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(15, 6), EnvironmentMeasure.Heat);
            ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(19, 9), EnvironmentMeasure.Heat);

            for (var i = 12; i < ecosystem.Width; i++)
            {
                for (var j = 4; j <= 5; j++)
                {
                    ecosystem.EnvironmentMeasureDistributor.InsertDistribution(new Coordinate(i, j), EnvironmentMeasure.Poison);
                }
            }

            for (var i = 0; i < 15; i++)
            {
                ecosystem.SetLevel(new Coordinate(i, 0), EnvironmentMeasure.Nutrient, 1.0 - (i * (1 / (double)15)));
                ecosystem.SetLevel(new Coordinate(i, 9), EnvironmentMeasure.Mineral, 1.0 - (i * (1 / (double)15)));
            }

            // custom obstructed habitats (will make a square shapen with an entrance - a pen?)
            var obstructedCoordinates = new List<Coordinate>
                                            {
                                                new Coordinate(1, 1),
                                                new Coordinate(1, 2),
                                                new Coordinate(1, 3),
                                                new Coordinate(1, 4),
                                                new Coordinate(1, 5),
                                                new Coordinate(1, 6),
                                                new Coordinate(1, 7),
                                                new Coordinate(1, 8),
                                                new Coordinate(2, 1),
                                                new Coordinate(3, 1),
                                                new Coordinate(4, 1),
                                                new Coordinate(5, 1),
                                                new Coordinate(6, 1),
                                                new Coordinate(7, 1),
                                                new Coordinate(2, 8),
                                                new Coordinate(3, 8),
                                                new Coordinate(4, 8),
                                                new Coordinate(5, 8),
                                                new Coordinate(6, 8),
                                                new Coordinate(7, 8),
                                                new Coordinate(8, 1),
                                                new Coordinate(8, 2),
                                                new Coordinate(8, 3),
                                                new Coordinate(8, 6),
                                                new Coordinate(8, 7),
                                                new Coordinate(8, 8)
                                            };

            foreach (var coordinate in obstructedCoordinates)
            {
                ecosystem.SetLevel(coordinate, EnvironmentMeasure.Obstruction, 1.0);
            }
        }

        protected virtual Dictionary<Organism, Coordinate> InitialOrganismCoordinates()
        {
            var soundEmittingOrganism = new Gatherer("Lotty", Colors.Silver);
            soundEmittingOrganism.EnableSound();

            var organismLocations = new Dictionary<Organism, Coordinate>
                                        {
                                            { new Gatherer("Waffle", Colors.Silver), new Coordinate(2, 2) },
                                            { new Gatherer("Wilber", Colors.Silver), new Coordinate(2, 7) },
                                            { soundEmittingOrganism, new Coordinate(7, 2) },
                                            { new Gatherer("Dr. Louise", Colors.Silver), new Coordinate(7, 7) },
                                        };

            return organismLocations;
        }
    }
}
