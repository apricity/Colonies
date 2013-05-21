namespace ColoniesTests
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies;
    using Colonies.Models;

    using ColoniesTests.Mocks;

    using NUnit.Framework;

    using Environment = Colonies.Models.Environment;

    [TestFixture]
    public class OrganismMovementTests
    {
        private Habitat[,] habitats;
        private Dictionary<string, Organism> organisms;
        private Dictionary<int, Coordinates> locations;

        [SetUp]
        public void SetupTest()
        {
            const int width = 10;
            const int height = 1;
            this.habitats = GenerateBaseHabitats(width, height);

            var organismIdentifiers = new List<string> { "A", "B", "C", "D", "W", "X", "Y", "Z" };

            this.organisms = new Dictionary<string, Organism>();
            foreach (var organismIdentifier in organismIdentifiers)
            {
                this.organisms.Add(organismIdentifier, new Organism(organismIdentifier, Color.Black, false));
            }

            this.locations = new Dictionary<int, Coordinates>();
            for (var i = 0; i < 10; i++)
            {
                this.locations.Add(i, new Coordinates(i, 0));
            }
        }

        [Test]
        public void IndependentMovements()
        {
            /* take a grid and populate with organisms: |___|_A_|___|_B_|___|___|___|___|___|___|
             * make A choose rightmost stimulus & make B choose leftmost stimulus
             * no conflict, both organisms should move where they chose to go
             * result of test:                          |_A_|___|___|___|_B_|___|___|___|___|___| */

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[1] },
                                            { this.organisms["B"], this.locations[3] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[0] },
                                            { this.organisms["B"], this.locations[4] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key, 
                    intendedDestination => intendedDestination.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
        }

        [Test]
        public void IndividualConflictingMovements()
        {
            /* take a grid and populate with organisms: |___|_B_|___|_A_|___|___|___|___|___|___|
             * make A choose leftmost stimulus & make B choose rightmost stimulus
             * make the ecosystem choose the first organism when >1 organism want to move to the same space
             * (therefore, when A wants to move left and B wants to move right, A will win)
             * result of test:                          |___|_B_|_A_|___|___|___|___|___|___|___| */

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[3] },
                                            { this.organisms["B"], this.locations[1] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[2] },
                                            { this.organisms["B"], this.locations[2] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            // expect B to not have moved
            expectedOrganismDestinations[this.organisms["B"]] = organismLocations[this.organisms["B"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
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

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[3] },
                                            { this.organisms["B"], this.locations[1] },
                                            { this.organisms["Y"], this.locations[6] },
                                            { this.organisms["Z"], this.locations[0] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[2] },
                                            { this.organisms["B"], this.locations[2] },
                                            { this.organisms["Y"], this.locations[7] },
                                            { this.organisms["Z"], this.locations[7] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            // expect B, Z to not have moved
            expectedOrganismDestinations[this.organisms["B"]] = organismLocations[this.organisms["B"]];
            expectedOrganismDestinations[this.organisms["Z"]] = organismLocations[this.organisms["Z"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
        }

        [Test]
        public void IndividualTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|_C_|_D_|___|___|___|___|___|___|
             * make A, B, C, D choose rightmost stimulus
             * all organisms are moving in convoy to the right, and each will be able to go to their desired destination
             * result of test:                          |___|_A_|_B_|_C_|_D_|___|___|___|___|___| */

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[0] },
                                            { this.organisms["B"], this.locations[1] },
                                            { this.organisms["C"], this.locations[2] },
                                            { this.organisms["D"], this.locations[3] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[1] },
                                            { this.organisms["B"], this.locations[2] },
                                            { this.organisms["C"], this.locations[3] },
                                            { this.organisms["D"], this.locations[4] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
        }

        [Test]
        public void MultipleTrailingMovements()
        {
            /* take a grid and populate with organisms: |_A_|_B_|_C_|_D_|___|_W_|_X_|_Y_|_Z_|___|
             * make A, B, C, D choose rightmost stimulus
             * make W, X, Y, Z choose rightmost stimulus
             * all organisms are moving in convoy to the right, and each will be able to go to their desired destination
             * result of test:                          |___|_A_|_B_|_C_|_D_|___|_W_|_X_|_Y_|_Z_| */

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[0] },
                                            { this.organisms["B"], this.locations[1] },
                                            { this.organisms["C"], this.locations[2] },
                                            { this.organisms["D"], this.locations[3] },
                                            { this.organisms["W"], this.locations[5] },
                                            { this.organisms["X"], this.locations[6] },
                                            { this.organisms["Y"], this.locations[7] },
                                            { this.organisms["Z"], this.locations[8] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[1] },
                                            { this.organisms["B"], this.locations[2] },
                                            { this.organisms["C"], this.locations[3] },
                                            { this.organisms["D"], this.locations[4] },
                                            { this.organisms["W"], this.locations[6] },
                                            { this.organisms["X"], this.locations[7] },
                                            { this.organisms["Y"], this.locations[8] },
                                            { this.organisms["Z"], this.locations[9] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
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

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[0] },
                                            { this.organisms["B"], this.locations[1] },
                                            { this.organisms["C"], this.locations[3] },
                                            { this.organisms["D"], this.locations[4] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[1] },
                                            { this.organisms["B"], this.locations[2] },
                                            { this.organisms["C"], this.locations[2] },
                                            { this.organisms["D"], this.locations[3] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            // expect C, D to not have moved
            expectedOrganismDestinations[this.organisms["C"]] = organismLocations[this.organisms["C"]];
            expectedOrganismDestinations[this.organisms["D"]] = organismLocations[this.organisms["D"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
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

            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[0] },
                                            { this.organisms["B"], this.locations[1] },
                                            { this.organisms["C"], this.locations[3] },
                                            { this.organisms["D"], this.locations[4] },
                                            { this.organisms["W"], this.locations[8] },
                                            { this.organisms["X"], this.locations[5] },
                                            { this.organisms["Y"], this.locations[6] },
                                            { this.organisms["Z"], this.locations[9] }
                                        };

            var organismIntendedDestinations = new Dictionary<Organism, Coordinates>
                                        {
                                            { this.organisms["A"], this.locations[1] },
                                            { this.organisms["B"], this.locations[2] },
                                            { this.organisms["C"], this.locations[2] },
                                            { this.organisms["D"], this.locations[3] },
                                            { this.organisms["W"], this.locations[7] },
                                            { this.organisms["X"], this.locations[6] },
                                            { this.organisms["Y"], this.locations[7] },
                                            { this.organisms["Z"], this.locations[8] }
                                        };

            var expectedOrganismDestinations = organismIntendedDestinations.ToDictionary(
                    intendedDestination => intendedDestination.Key,
                    intendedDestination => intendedDestination.Value);

            // expect C, D, X, Y to not have moved
            expectedOrganismDestinations[this.organisms["C"]] = organismLocations[this.organisms["C"]];
            expectedOrganismDestinations[this.organisms["D"]] = organismLocations[this.organisms["D"]];
            expectedOrganismDestinations[this.organisms["X"]] = organismLocations[this.organisms["X"]];
            expectedOrganismDestinations[this.organisms["Y"]] = organismLocations[this.organisms["Y"]];

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations, organismIntendedDestinations);

            var expectedCoordinates = expectedOrganismDestinations.Values.ToList();
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            Assert.That(actualCoordinates, Is.EqualTo(expectedCoordinates));
        }

        private static Habitat[,] GenerateBaseHabitats(int width, int height)
        {
            var habitats = new Habitat[width,height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var environment = new Environment(Terrain.Unknown);
                    habitats[x, y] = new Habitat(environment, null);
                }
            }

            return habitats;
        }

        private UpdateSummary CreateAndUpdateEcosystem(Dictionary<Organism, Coordinates> organismLocations, Dictionary<Organism, Coordinates> organismIntendedDestinations)
        {
            foreach (var organismLocation in organismLocations)
            {
                var organism = organismLocation.Key;
                var location = organismLocation.Value;

                this.habitats[location.X, location.Y].AddOrganism(organism);
            }

            var mockEcosystem = new MockEcosystem(habitats, organismLocations, organismIntendedDestinations);
            return mockEcosystem.Update();
        }
    }
}
