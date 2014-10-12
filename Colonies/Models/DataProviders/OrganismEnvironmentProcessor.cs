namespace Wacton.Colonies.Models.DataProviders
{
    using System;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class OrganismEnvironmentProcessor
    {
        private readonly IEcosystemData ecosystemData;

        public OrganismEnvironmentProcessor(IEcosystemData ecosystemData)
        {
            this.ecosystemData = ecosystemData;
        }

        public EcosystemModification Process(Coordinate organismCoordinate)
        {
            var ecosystemModification = new EcosystemModification();
            ecosystemModification.Add(this.ProcessInventoryNutrient(organismCoordinate));
            ecosystemModification.Add(this.ProcessEnvironmentNutrient(organismCoordinate));
            ecosystemModification.Add(this.ProcessEnvironmentMineral(organismCoordinate));
            ecosystemModification.Add(this.ProcessEnvironmentHazards(organismCoordinate));
            return ecosystemModification;
        }

        private EcosystemModification ProcessInventoryNutrient(Coordinate organismCoordinate)
        {
            var ecosystemModification = new EcosystemModification();

            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            if (!organism.Intention.Equals(Intention.Eat) || !organism.Inventory.Equals(Inventory.Nutrient))
            {
                return ecosystemModification;
            }

            var availableInventoryNutrient = organism.GetLevel(OrganismMeasure.Inventory);
            var desiredInventoryNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
            var inventoryNutrientTaken = Math.Min(desiredInventoryNutrient, availableInventoryNutrient);

            ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Health, inventoryNutrientTaken));
            ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Inventory, -inventoryNutrientTaken));
            return ecosystemModification;
        }

        private EcosystemModification ProcessEnvironmentNutrient(Coordinate organismCoordinate)
        {
            var ecosystemModification = new EcosystemModification();

            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);
            var availableNutrient = environment.GetLevel(EnvironmentMeasure.Nutrient);

            if (availableNutrient.Equals(0.0))
            {
                return ecosystemModification;
            }

            if (organism.Intention.Equals(Intention.Harvest))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Inventory, nutrientTaken));
                ecosystemModification.Add(new EnvironmentModification(organismCoordinate, EnvironmentMeasure.Nutrient, -nutrientTaken));
            }

            if (organism.Intention.Equals(Intention.Eat))
            {
                var desiredNutrient = 1 - organism.GetLevel(OrganismMeasure.Health);
                var nutrientTaken = Math.Min(desiredNutrient, availableNutrient);
                ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Health, nutrientTaken));
                ecosystemModification.Add(new EnvironmentModification(organismCoordinate, EnvironmentMeasure.Nutrient, -nutrientTaken));
            }

            return ecosystemModification;
        }

        private EcosystemModification ProcessEnvironmentMineral(Coordinate organismCoordinate)
        {
            var ecosystemModification = new EcosystemModification();

            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);

            var availableMineral = environment.GetLevel(EnvironmentMeasure.Mineral);

            if (availableMineral.Equals(0.0))
            {
                return ecosystemModification;
            }

            if (organism.Intention.Equals(Intention.Mine))
            {
                var desiredMineral = 1 - organism.GetLevel(OrganismMeasure.Inventory);
                var mineralTaken = Math.Min(desiredMineral, availableMineral);
                ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Inventory, mineralTaken));
                ecosystemModification.Add(new EnvironmentModification(organismCoordinate, EnvironmentMeasure.Mineral, -mineralTaken));
            }

            // reproduction requirements (first pass: mineral level 1.0, health level 0.75)
            if (organism.Intention.Equals(Intention.Reproduce)
                && environment.GetLevel(EnvironmentMeasure.Mineral).Equals(1.0)
                && organism.GetLevel(OrganismMeasure.Health) > 0.75)
            {
                // TODO: create the result of using the mineral during reproduction!  a child organism?!
                var mineralTaken = availableMineral;
                ecosystemModification.Add(new EnvironmentModification(organismCoordinate, EnvironmentMeasure.Mineral, -mineralTaken));
            }

            return ecosystemModification;
        }

        private EcosystemModification ProcessEnvironmentHazards(Coordinate organismCoordinate)
        {
            var ecosystemModification = new EcosystemModification();

            var organism = this.ecosystemData.GetOrganism(organismCoordinate);
            var environment = this.ecosystemData.GetEnvironment(organismCoordinate);

            var hazardousMeasurements = environment.MeasurementData.Measurements.Where(measurement => measurement.Measure.IsHazardous).ToList();

            if (!organism.Intention.Equals(Intention.Build)
                || organism.GetLevel(OrganismMeasure.Inventory) < 1.0)
            {
                return ecosystemModification;
            }

            if (hazardousMeasurements.Any(measurement => measurement.Level > 0.0))
            {
                ecosystemModification.Add(new OrganismModification(organismCoordinate, OrganismMeasure.Inventory, -1.0));
                ecosystemModification.Add(new EnvironmentModification(organismCoordinate, EnvironmentMeasure.Obstruction, 1.0));
            }

            return ecosystemModification;
        }
    }
}
