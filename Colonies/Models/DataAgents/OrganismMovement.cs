namespace Wacton.Colonies.Models.DataAgents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;

    public class OrganismMovement : IEcosystemStage, IBiased<OrganismMeasure>
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemRates ecosystemRates;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;

        public Dictionary<OrganismMeasure, double> MeasureBiases { get; private set; }

        public Dictionary<IOrganism, Coordinate> OverrideDesiredOrganismCoordinates { get; set; }
        public Func<IEnumerable<IOrganism>, IOrganism> OverrideDecideOrganismFunction { get; set; } 

        public OrganismMovement(EcosystemData ecosystemData, EcosystemRates ecosystemRates, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemRates = ecosystemRates;
            this.environmentMeasureDistributor = environmentMeasureDistributor;

            this.MeasureBiases = new Dictionary<OrganismMeasure, double>
                                     {
                                         { OrganismMeasure.Health, 1.0 },
                                         { OrganismMeasure.Inventory, 0.0 }
                                     };
        }

        public void Execute()
        {
            var desiredOrganismCoordinates = this.GetDesiredCoordinates();
            var resolvedOrganismCoordinates = this.ResolveOrganismHabitats(desiredOrganismCoordinates, new List<IOrganism>());

            this.IncreasePheromoneLevels();
            this.IncreaseMineralLevels();

            foreach (var organismCoordinate in resolvedOrganismCoordinates)
            {
                this.ecosystemData.MoveOrganism(organismCoordinate.Key, organismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            this.ecosystemData.AdjustLevels(obstructedCoordinates, EnvironmentMeasure.Obstruction, -this.ecosystemRates.DecreasingRates[EnvironmentMeasure.Obstruction]);

            this.InsertSoundDistribution();
        }

        public Dictionary<IOrganism, Coordinate> GetDesiredCoordinates()
        {
            if (this.OverrideDesiredOrganismCoordinates != null)
            {
                return this.OverrideDesiredOrganismCoordinates;
            }

            var desiredOrganismCoordinates = new Dictionary<IOrganism, Coordinate>();
            var aliveOrganismCoordinates = this.ecosystemData.AliveOrganismCoordinates().ToList();

            foreach (var organismCoordinate in aliveOrganismCoordinates)
            {
                // remain stationary if organism is reproducing
                var organism = this.ecosystemData.GetOrganism(organismCoordinate);
                if (organism.CurrentIntention.Equals(Intention.Reproduce))
                {
                    desiredOrganismCoordinates.Add(organism, organismCoordinate);
                    continue;
                }

                // get measurements of neighbouring environments
                var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(organismCoordinate, 1, false, true).ToList();

                // determine organism's intentions based on the environment measurements
                var measurableItems = neighbourCoordinates.Select(this.ecosystemData.GetEnvironment).ToList();
                var biasProvider = this.ecosystemData.GetOrganism(organismCoordinate);
                var chosenMeasurable = DecisionLogic.MakeDecision(measurableItems, biasProvider);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenCoordinate = neighbourCoordinates.Single(coordinate => this.ecosystemData.GetEnvironment(coordinate).Equals(chosenMeasurable));
                desiredOrganismCoordinates.Add(organism, chosenCoordinate);
            }

            return desiredOrganismCoordinates;
        }

        public Dictionary<IOrganism, Coordinate> ResolveOrganismHabitats(Dictionary<IOrganism, Coordinate> desiredOrganismCoordinates, IEnumerable<IOrganism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismCoordinates = new Dictionary<IOrganism, Coordinate>();

            // create a copy of the organism habitats because we don't want to modify the actual set
            var organisms = this.ecosystemData.OrganismCoordinates().Select(this.ecosystemData.GetOrganism).ToList();

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                organisms.Remove(alreadyResolvedOrganism);
            }

            var occupiedCoordinates = organisms.Select(this.ecosystemData.CoordinateOf).ToList();
            var desiredCoordinates = desiredOrganismCoordinates.Values;

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantCoordinates = desiredCoordinates
                .Except(occupiedCoordinates)
                .Where(coordinate => !this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction))
                .Distinct().ToList();
            if (vacantCoordinates.Count == 0)
            {
                return resolvedOrganismCoordinates;
            }

            foreach (var coordinate in vacantCoordinates)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantCoordinate = coordinate;
                var conflictingOrganisms = desiredOrganismCoordinates
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantCoordinate))
                    .Select(intendedOrganismDestination => intendedOrganismDestination.Key)
                    .ToList();

                IOrganism organismToMove;
                if (conflictingOrganisms.Count > 1)
                {
                    organismToMove = this.DecideOrganism(conflictingOrganisms);
                    conflictingOrganisms.Remove(organismToMove);

                    // the remaining conflicting organisms cannot move, so reset their intended destinations
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        desiredOrganismCoordinates[remainingOrganism] = this.ecosystemData.CoordinateOf(remainingOrganism);
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }

                // intended movement becomes an actual, resolved movement
                resolvedOrganismCoordinates.Add(organismToMove, desiredOrganismCoordinates[organismToMove]);
                desiredOrganismCoordinates.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismCoordinates.Keys.ToList();
            var trailingOrganismHabitats = this.ResolveOrganismHabitats(desiredOrganismCoordinates, resolvedOrganisms);
            foreach (var trailingOrganismHabitat in trailingOrganismHabitats)
            {
                resolvedOrganismCoordinates.Add(trailingOrganismHabitat.Key, trailingOrganismHabitat.Value);
            }

            return resolvedOrganismCoordinates;
        }

        private IOrganism DecideOrganism(IEnumerable<IOrganism> organisms)
        {
            if (this.OverrideDecideOrganismFunction != null)
            {
                return this.OverrideDecideOrganismFunction.Invoke(organisms);
            }

            return DecisionLogic.MakeDecision(organisms, this);
        }

        private void IncreasePheromoneLevels()
        {
            var organismCoordinates = this.ecosystemData.DepositingPheromoneOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, EnvironmentMeasure.Pheromone, this.ecosystemRates.IncreasingRates[EnvironmentMeasure.Pheromone]);
        }

        private void IncreaseMineralLevels()
        {
            // only increase mineral where the terrain is not harmful (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var organismCoordinates = this.ecosystemData.OrganismCoordinates()
                .Where(coordinate => !this.ecosystemData.IsHarmful(coordinate)).ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, EnvironmentMeasure.Mineral, this.ecosystemRates.IncreasingRates[EnvironmentMeasure.Mineral]);
        }

        private void InsertSoundDistribution()
        {
            foreach (var organismCoordinate in this.ecosystemData.AudibleOrganismCoordinates())
            {
                this.environmentMeasureDistributor.InsertDistribution(organismCoordinate, EnvironmentMeasure.Sound);
            }
        }
    }
}