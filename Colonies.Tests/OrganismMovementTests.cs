namespace Wacton.Colonies.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using NUnit.Framework;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems;
    using Wacton.Colonies.Domain.Ecosystems.Data;
    using Wacton.Colonies.Domain.Ecosystems.Modification;
    using Wacton.Colonies.Domain.Ecosystems.Phases;
    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Plugins;
    using Wacton.Colonies.Domain.Weathers;

    using Environment = Wacton.Colonies.Domain.Environments.Environment;

    [TestFixture]
    public class OrganismMovementTests
    {
        private Habitat[,] habitats;
        private Dictionary<string, Organism> organismsById;
        private Dictionary<Habitat, Coordinate> habitatCoordinates;

        [SetUp]
        public void SetupTest()
        {
            const int Width = 10;
            const int Height = 1;
            this.habitats = GenerateBaseHabitats(Width, Height);

            var organismIdentifiers = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };

            this.organismsById = new Dictionary<string, Organism>();
            foreach (var organismIdentifier in organismIdentifiers)
            {
                this.organismsById.Add(organismIdentifier, new Organism(Guid.NewGuid(), organismIdentifier, Colors.Black, new DummyLogic()));
            }

            this.habitatCoordinates = new Dictionary<Habitat, Coordinate>();
            for (var i = 0; i < this.habitats.GetLength(0); i++)
            {
                for (var j = 0; j < this.habitats.GetLength(1); j++)
                {
                    this.habitatCoordinates.Add(this.habitats[i, j], new Coordinate(i, j));
                }
            }
        }

        [Test]
        public void IndependentMovements()
        {
            /* take a grid and populate with organisms: |___|_A_|___|_B_|___|___|___|___|___|___|
             * make A choose rightmost stimulus & make B choose leftmost stimulus
             * no conflict, both organisms should move where they chose to go
             * result of test:                          |_A_|___|___|___|_B_|___|___|___|___|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(1, 0) },
                                            { this.organismsById["B"], new Coordinate(3, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(0, 0) },
                                            { this.organismsById["B"], new Coordinate(4, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void IndividualConflictingMovements()
        {
            /* take a grid and populate with organisms: |___|_B_|___|_A_|___|___|___|___|___|___|
             * make A choose leftmost stimulus & make B choose rightmost stimulus
             * make the ecosystem choose the first organism when >1 organism want to move to the same space
             * (therefore, when A wants to move left and B wants to move right, A will win)
             * result of test:                          |___|_B_|_A_|___|___|___|___|___|___|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(3, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(2, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            // expect B to not have moved
            expectedOrganismCoordinates[this.organismsById["B"]] = organismCoordinates[this.organismsById["B"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void MultipleConflictingMovements()
        {
            /* take a grid and populate with organisms: |___|_B_|___|_A_|___|___|_Y_|___|_Z_|___|
             * make A choose leftmost stimulus & make B choose rightmost stimulus
             * make Y choose rightmost stimulus & make Z choose leftmost stimulus 
             * make the ecosystem choose the first organism when >1 organism want to move to the same space
             * (therefore, when A wants to move left and B wants to move right, A will win)
             * (therefore, when Y wants to move right and Z wants to move left, Y will win)
             * result of test:                          |___|_B_|_A_|___|___|___|___|_Y_|_Z_|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(3, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) },
                                            { this.organismsById["Y"], new Coordinate(6, 0) },
                                            { this.organismsById["Z"], new Coordinate(0, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(2, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) },
                                            { this.organismsById["Y"], new Coordinate(7, 0) },
                                            { this.organismsById["Z"], new Coordinate(7, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            // expect B, Z to not have moved
            expectedOrganismCoordinates[this.organismsById["B"]] = organismCoordinates[this.organismsById["B"]];
            expectedOrganismCoordinates[this.organismsById["Z"]] = organismCoordinates[this.organismsById["Z"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void IndividualTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|_C_|_D_|___|___|___|___|___|___|
             * make A, B, C, D choose rightmost stimulus
             * all organisms are moving in convoy to the right, and each will be able to go to their desired destination
             * result of test:                          |___|_A_|_B_|_C_|_D_|___|___|___|___|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(0, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) },
                                            { this.organismsById["C"], new Coordinate(2, 0) },
                                            { this.organismsById["D"], new Coordinate(3, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(1, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) },
                                            { this.organismsById["C"], new Coordinate(3, 0) },
                                            { this.organismsById["D"], new Coordinate(4, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void MultipleTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|_C_|_D_|___|_W_|_X_|_Y_|_Z_|___|
             * make A, B, C, D choose rightmost stimulus
             * make W, X, Y, Z choose rightmost stimulus
             * all organisms are moving in convoy to the right, and each will be able to go to their desired destination
             * result of test:                          |___|_A_|_B_|_C_|_D_|___|_W_|_X_|_Y_|_Z_| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(0, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) },
                                            { this.organismsById["C"], new Coordinate(2, 0) },
                                            { this.organismsById["D"], new Coordinate(3, 0) },
                                            { this.organismsById["W"], new Coordinate(5, 0) },
                                            { this.organismsById["X"], new Coordinate(6, 0) },
                                            { this.organismsById["Y"], new Coordinate(7, 0) },
                                            { this.organismsById["Z"], new Coordinate(8, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(1, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) },
                                            { this.organismsById["C"], new Coordinate(3, 0) },
                                            { this.organismsById["D"], new Coordinate(4, 0) },
                                            { this.organismsById["W"], new Coordinate(6, 0) },
                                            { this.organismsById["X"], new Coordinate(7, 0) },
                                            { this.organismsById["Y"], new Coordinate(8, 0) },
                                            { this.organismsById["Z"], new Coordinate(9, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void IndividualConflictingAndTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|___|_C_|_D_|___|___|___|___|___|
             * make A, B choose rightmost stimulus & make C, D choose leftmost stimulus
             * make the ecosystem choose the first organism when >1 organism want to move to the same space
             * (therefore, when B wants to move right and C wants to move left, B will win)
             * A is trailing B, and will be able to move when B wins the vacant destination
             * result of test:                          |___|_A_|_B_|_C_|_D_|___|___|___|___|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(0, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) },
                                            { this.organismsById["C"], new Coordinate(3, 0) },
                                            { this.organismsById["D"], new Coordinate(4, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(1, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) },
                                            { this.organismsById["C"], new Coordinate(2, 0) },
                                            { this.organismsById["D"], new Coordinate(3, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            // expect C, D to not have moved
            expectedOrganismCoordinates[this.organismsById["C"]] = organismCoordinates[this.organismsById["C"]];
            expectedOrganismCoordinates[this.organismsById["D"]] = organismCoordinates[this.organismsById["D"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        [Test]
        public void MultipleConflictingAndTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|___|_C_|_D_|_X_|_Y_|___|_W_|_Z_|
             * make A, B choose rightmost stimulus & make C, D choose leftmost stimulus
             * make X, Y choose rightmost stimulus & make W, Z choose leftmost stimulus
             * make the ecosystem choose the first organism when >1 organism want to move to the same space
             * (therefore, when B wants to move right and C wants to move left, B will win)
             * (therefore, when W wants to move left and Y wants to move right, W will win) 
             * A is trailing B, and will be able to move when B wins the vacant destination
             * Z is trailing W, and will be able to move when W wins the vacant destination 
             * result of test:                          |___|_A_|_B_|_C_|_D_|_X_|_Y_|_W_|_Z_|___| */

            var organismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(0, 0) },
                                            { this.organismsById["B"], new Coordinate(1, 0) },
                                            { this.organismsById["C"], new Coordinate(3, 0) },
                                            { this.organismsById["D"], new Coordinate(4, 0) },
                                            { this.organismsById["W"], new Coordinate(8, 0) },
                                            { this.organismsById["X"], new Coordinate(5, 0) },
                                            { this.organismsById["Y"], new Coordinate(6, 0) },
                                            { this.organismsById["Z"], new Coordinate(9, 0) }
                                        };

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { this.organismsById["A"], new Coordinate(1, 0) },
                                            { this.organismsById["B"], new Coordinate(2, 0) },
                                            { this.organismsById["C"], new Coordinate(2, 0) },
                                            { this.organismsById["D"], new Coordinate(3, 0) },
                                            { this.organismsById["W"], new Coordinate(7, 0) },
                                            { this.organismsById["X"], new Coordinate(6, 0) },
                                            { this.organismsById["Y"], new Coordinate(7, 0) },
                                            { this.organismsById["Z"], new Coordinate(8, 0) }
                                        };

            var expectedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => organismCoordinate.Value);

            // expect C, D, X, Y to not have moved
            expectedOrganismCoordinates[this.organismsById["C"]] = organismCoordinates[this.organismsById["C"]];
            expectedOrganismCoordinates[this.organismsById["D"]] = organismCoordinates[this.organismsById["D"]];
            expectedOrganismCoordinates[this.organismsById["X"]] = organismCoordinates[this.organismsById["X"]];
            expectedOrganismCoordinates[this.organismsById["Y"]] = organismCoordinates[this.organismsById["Y"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismCoordinates, desiredOrganismCoordinates);
            Assert.AreEqual(updateSummary.OrganismCoordinates, expectedOrganismCoordinates);
        }

        private static Habitat[,] GenerateBaseHabitats(int width, int height)
        {
            var habitats = new Habitat[width,height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var environment = new Environment();
                    habitats[x, y] = new Habitat(environment, null);
                }
            }

            return habitats;
        }

        private PhaseSummary CreateAndUpdateEcosystem(Dictionary<IOrganism, Coordinate> organismCoordinates, Dictionary<IOrganism, Coordinate> desiredOrganismCoordinates)
        {
            var desiredBiasedOrganismCoordinates = desiredOrganismCoordinates.ToDictionary(
                desiredOrganismCoordinate => desiredOrganismCoordinate.Key,
                desiredOrganismCoordinate => desiredOrganismCoordinate.Value);

            var organismFactory = new OrganismFactory(new List<ColonyPluginData>());
            var ecosystemHistory = new EcosystemHistory();
            var ecosystemData = new EcosystemData(this.habitats, organismCoordinates, ecosystemHistory);
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
            var ecosystemStages = new EcosystemPhases(new List<IEcosystemPhase> { setupPhase, actionPhase, interactionPhase, movementPhase, ambientPhase });
            var ecosystem = new Ecosystem(ecosystemData, ecosystemRates, ecosystemHistory, weather, distributor, ecosystemStages);

            movementPhase.OverrideDesiredOrganismCoordinates = desiredBiasedOrganismCoordinates;
            movementPhase.OverrideDecideOrganismFunction = organisms => organisms.First();

            var setupUpdateSummary = ecosystem.ExecuteOnePhase();
            var actionUpdateSummary = ecosystem.ExecuteOnePhase();
            var interactionUpdateSummary = ecosystem.ExecuteOnePhase();
            var movementUpdateSummary = ecosystem.ExecuteOnePhase();
            return movementUpdateSummary;
        }
    }
}
