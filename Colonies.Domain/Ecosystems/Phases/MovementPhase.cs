namespace Wacton.Colonies.Domain.Ecosystems.Phases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems.Modification;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.Domain.Settings;

    public class MovementPhase : IEcosystemPhase, IBiased<OrganismMeasure>
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemSettings ecosystemSettings;
        public Dictionary<OrganismMeasure, double> MeasureBiases { get; }

        public Dictionary<IOrganism, Coordinate> OverrideDesiredOrganismCoordinates { get; set; }
        public Func<IEnumerable<IOrganism>, IOrganism> OverrideDecideOrganismFunction { get; set; } 

        public MovementPhase(EcosystemData ecosystemData, EcosystemSettings ecosystemSettings)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemSettings = ecosystemSettings;

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

            foreach (var organismCoordinate in resolvedOrganismCoordinates)
            {
                var organism = organismCoordinate.Key;
                var preMoveCoordinate = this.ecosystemData.CoordinateOf(organism);
                var postMoveCoordinate = organismCoordinate.Value;
                this.ecosystemData.MoveOrganism(organism, postMoveCoordinate);
                this.IncreaseMineralLevel(preMoveCoordinate);
            }

            // increase mineral wherever there are dead organisms
            var deadOrganismCoordinates = this.ecosystemData.DeadOrganismCoordinates().Where(coordinate => !this.ecosystemData.IsHarmful(coordinate)).ToList();
            foreach (var organismCoordinate in deadOrganismCoordinates)
            {
                this.IncreaseMineralLevel(organismCoordinate);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            this.ecosystemData.AdjustLevels(obstructedCoordinates, EnvironmentMeasure.Obstruction, -this.ecosystemSettings.DecreasingRates[EnvironmentMeasure.Obstruction]);
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
                if (!organism.CanMove)
                {
                    desiredOrganismCoordinates.Add(organism, organismCoordinate);
                    continue;
                }

                // get measurements of neighbouring environments
                var neighbourCoordinates = this.ecosystemData.GetValidNeighbours(organismCoordinate, 1, false, true).ToList();

                // determine organism's intentions based on the environment measurements
                var measurableItems = neighbourCoordinates.Select(this.ecosystemData.GetEnvironment).ToList();
                var biasProvider = this.ecosystemData.GetOrganism(organismCoordinate);
                var chosenMeasurable = Decider.MakeDecision(measurableItems, biasProvider);

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

            return Decider.MakeDecision(organisms, this);
        }

        private void IncreasePheromoneLevels()
        {
            var organismCoordinates = this.ecosystemData.DepositingPheromoneOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, EnvironmentMeasure.Pheromone, this.ecosystemSettings.IncreasingRates[EnvironmentMeasure.Pheromone]);
        }

        private void IncreaseMineralLevel(Coordinate coordinate)
        {
            // only increase if the environment is not harmful
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            if (this.ecosystemData.IsHarmful(coordinate))
            {
                return;
            }

            this.ecosystemData.AdjustLevel(coordinate, EnvironmentMeasure.Mineral, this.ecosystemSettings.IncreasingRates[EnvironmentMeasure.Mineral]);
        }
    }
}