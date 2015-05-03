namespace Wacton.Colonies.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Ecosystems.Modification;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Environments;
    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Mains;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.OrganismSynopses;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Weathers;

    public class DomainBootstrapper
    {
        public Main BuildDomainModel(int ecosystemWidth, int ecosystemHeight)
        {
            var habitats = new Habitat[ecosystemWidth, ecosystemHeight];

            for (var x = 0; x < ecosystemWidth; x++)
            {
                for (var y = 0; y < ecosystemHeight; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment();
                    var habitat = new Habitat(environment, null);
                    habitats[x, y] = habitat;
                }
            }

            var organismFactory = new OrganismFactory();
            var initialOrganismCoordinates = this.InitialOrganismCoordinates(organismFactory);
            var ecosystemHistory = new EcosystemHistory();
            var ecosystemData = new EcosystemData(habitats, initialOrganismCoordinates, ecosystemHistory);
            var ecosystemRates = new EcosystemRates();
            var weather = new Weather();
            var distributor = new Distributor(ecosystemData);
            var afflictor = new Afflictor(ecosystemData, distributor);
            var hazardFlow = new HazardFlow(ecosystemData, ecosystemRates, distributor, weather);
            var setupPhase = new SetupPhase(ecosystemData, afflictor);
            var actionPhase = new ActionPhase(ecosystemData);
            var movementPhase = new MovementPhase(ecosystemData, ecosystemRates);
            var interactionPhase = new InteractionPhase(ecosystemData, organismFactory, afflictor);
            var ambientPhase = new AmbientPhase(ecosystemData, ecosystemRates, distributor, weather, hazardFlow);
            var ecosystemPhases = new EcosystemPhases(new List<IEcosystemPhase> { setupPhase, actionPhase, interactionPhase, movementPhase, ambientPhase });
            var ecosystem = new Ecosystem(ecosystemData, ecosystemRates, ecosystemHistory, weather, distributor, ecosystemPhases);

            this.InitialiseTerrain(ecosystem);
            foreach (var organismCoordinate in ecosystemData.AudibleOrganismCoordinates())
            {
                ecosystem.Distributor.Insert(EnvironmentMeasure.Sound, organismCoordinate);
            }

            // hook organism model into the organism synopsis
            var organismSynopsis = new OrganismSynopsis(initialOrganismCoordinates.Keys.Select(organism => (IOrganism)organism).ToList());
            var main = new Main(ecosystem, organismSynopsis);

            // clear the history so this setup is not shown in the first pull of the history
            ecosystemHistory.Pull();

            return main;
        }

        protected virtual void InitialiseTerrain(Ecosystem ecosystem)
        {
            ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(19, 0));
            ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(15, 3));
            ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(17, 4));
            ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(17, 5));
            ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(15, 6));
            ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(19, 9));

            // testing hazard combinations
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(2, 2));
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Disease, new Coordinate(2, 2));

            //ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(2, 7));
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Disease, new Coordinate(2, 7));

            //ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(7, 2));
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(7, 2));

            //ecosystem.Distributor.Insert(EnvironmentMeasure.Heat, new Coordinate(7, 7));
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Damp, new Coordinate(7, 7));
            //ecosystem.Distributor.Insert(EnvironmentMeasure.Disease, new Coordinate(7, 7));

            for (var i = 12; i < ecosystem.Width; i++)
            {
                for (var j = 4; j <= 5; j++)
                {
                    ecosystem.Distributor.Insert(EnvironmentMeasure.Disease, new Coordinate(i, j));
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
                                                new Coordinate(8, 1),
                                                new Coordinate(8, 2),
                                                new Coordinate(8, 3),
                                                new Coordinate(8, 4),
                                                new Coordinate(8, 5),
                                                new Coordinate(8, 6),
                                                new Coordinate(8, 7),
                                                new Coordinate(8, 8)
                                            };

            foreach (var coordinate in obstructedCoordinates)
            {
                ecosystem.SetLevel(coordinate, EnvironmentMeasure.Obstruction, 1.0);
            }
        }

        protected virtual Dictionary<IOrganism, Coordinate> InitialOrganismCoordinates(OrganismFactory organismFactory)
        {
            var firstQueen = organismFactory.CreateOrphanOrganism(Colors.Silver, typeof(Queen));
            var organismLocations = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { organismFactory.CreateOffspringOrganism(firstQueen), new Coordinate(2, 2) },
                                            { organismFactory.CreateOffspringOrganism(firstQueen), new Coordinate(2, 7) },
                                            { organismFactory.CreateOffspringOrganism(firstQueen), new Coordinate(7, 2) },
                                            { firstQueen, new Coordinate(7, 7) },
                                        };

            return organismLocations;
        }
    }
}
