namespace ColoniesTests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies;
    using Colonies.Logic;
    using Colonies.Models;

    using Moq;

    using NUnit.Framework;

    using Environment = Colonies.Models.Environment;

    [TestFixture]
    public class OrganismMovementTests
    {
        private Habitat[,] habitats;
        private Mock<IStimuliProcessingLogic> rightmostStimulusLogic;
        private Mock<IStimuliProcessingLogic> leftmostStimulusLogic;

        [SetUp]
        public void SetupTest()
        {
            const int width = 10;
            const int height = 1;
            this.habitats = GenerateBaseHabitats(width, height);

            // mock stimuli processing logic; one returns the rightmost stimulus, the other returns the leftmost stimulus
            // these are used by organisms to make decisions about what stimuli to follow
            // (stimuli are generated left-to-right, top-to-bottom => left is first, right is last)
            this.rightmostStimulusLogic = new Mock<IStimuliProcessingLogic>();
            this.rightmostStimulusLogic.Setup(logic => logic.ProcessStimuli(It.IsAny<IEnumerable<Stimulus>>(), It.IsAny<double>(), It.IsAny<Random>()))
                                       .Returns((IEnumerable<Stimulus> stimuli, double pheromoneWeighting, Random random) => stimuli.Last());

            this.leftmostStimulusLogic = new Mock<IStimuliProcessingLogic>();
            this.leftmostStimulusLogic.Setup(logic => logic.ProcessStimuli(It.IsAny<IEnumerable<Stimulus>>(), It.IsAny<double>(), It.IsAny<Random>()))
                                      .Returns((IEnumerable<Stimulus> stimuli, double pheromoneWeighting, Random random) => stimuli.First());
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
                                            { new Organism("A", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(3, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(0, 0),
                                              new Coordinates(4, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(3, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(2, 0),
                                              new Coordinates(1, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(3, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("Y", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(6, 0) },
                                            { new Organism("Z", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(8, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(2, 0),
                                              new Coordinates(1, 0),
                                              new Coordinates(7, 0),
                                              new Coordinates(8, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(0, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("C", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(2, 0) },
                                            { new Organism("D", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(3, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(1, 0),
                                              new Coordinates(2, 0),
                                              new Coordinates(3, 0),
                                              new Coordinates(4, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(0, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("C", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(2, 0) },
                                            { new Organism("D", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(3, 0) },
                                            { new Organism("W", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(5, 0) },
                                            { new Organism("X", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(6, 0) },
                                            { new Organism("Y", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(7, 0) },
                                            { new Organism("Z", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(8, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(1, 0),
                                              new Coordinates(2, 0),
                                              new Coordinates(3, 0),
                                              new Coordinates(4, 0),
                                              new Coordinates(6, 0),
                                              new Coordinates(7, 0),
                                              new Coordinates(8, 0),
                                              new Coordinates(9, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(0, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("C", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(3, 0) },
                                            { new Organism("D", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(4, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(1, 0),
                                              new Coordinates(2, 0),
                                              new Coordinates(3, 0),
                                              new Coordinates(4, 0)
                                          };

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
                                            { new Organism("A", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(0, 0) },
                                            { new Organism("B", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(1, 0) },
                                            { new Organism("C", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(3, 0) },
                                            { new Organism("D", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(4, 0) },
                                            { new Organism("W", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(8, 0) },
                                            { new Organism("X", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(5, 0) },
                                            { new Organism("Y", Color.Black, this.rightmostStimulusLogic.Object, false), new Coordinates(6, 0) },
                                            { new Organism("Z", Color.Black, this.leftmostStimulusLogic.Object, false), new Coordinates(9, 0) }
                                        };

            var updateSummary = this.CreateAndUpdateEcosystem(organismLocations);
            var actualCoordinates = updateSummary.PostUpdateSummary.Values.ToList();
            var expectedCoordinates = new List<Coordinates>
                                          {
                                              new Coordinates(1, 0),
                                              new Coordinates(2, 0),
                                              new Coordinates(3, 0),
                                              new Coordinates(4, 0),
                                              new Coordinates(7, 0),
                                              new Coordinates(5, 0),
                                              new Coordinates(6, 0),
                                              new Coordinates(8, 0)
                                          };

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

        private UpdateSummary CreateAndUpdateEcosystem(Dictionary<Organism, Coordinates> organismLocations)
        {
            foreach (var organismLocation in organismLocations)
            {
                var organism = organismLocation.Key;
                var location = organismLocation.Value;

                this.habitats[location.X, location.Y].AddOrganism(organism);
            }

            // mock conflicting movement logic that awards the desired space to the first organism in the list
            // the ecosystem's lists of organisms are kept in order of creation, so the earliest-created organisms will win
            var conflictingMovementLogic = new Mock<IConflictingMovementLogic>();
            conflictingMovementLogic.Setup(logic => logic.DecideOrganism(It.IsAny<List<Organism>>(), It.IsAny<double>(), It.IsAny<Random>()))
                                    .Returns((List<Organism> organisms, double healthWeighting, Random random) => organisms.First());

            var ecosystem = new Ecosystem(habitats, organismLocations, conflictingMovementLogic.Object);
            return ecosystem.Update();
        }
    }
}
