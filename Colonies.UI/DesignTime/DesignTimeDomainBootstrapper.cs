namespace Wacton.Colonies.UI.DesignTime
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Domain;
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Ecosystems;
    using Wacton.Colonies.Domain.Mains;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;

    public class DesignTimeDomainBootstrapper : DomainBootstrapper
    {
        private const int EcosystemWidth = 10;
        private const int EcosystemHeight = 10;

        public Main Domain { get; private set; }

        public void Run()
        {
            this.Domain = this.BuildDomainModel(EcosystemWidth, EcosystemHeight);
        }

        protected override void InitialiseTerrain(Ecosystem ecosystem)
        {
            ecosystem.SetLevel(new Coordinate(1, 1), EnvironmentMeasure.Obstruction, 1.0);
            ecosystem.SetLevel(new Coordinate(1, 1), EnvironmentMeasure.Sound, 0.5);
        }

        protected override Dictionary<IOrganism, Coordinate> InitialOrganismCoordinates(OrganismFactory organismFactory)
        {
            var guid = Guid.NewGuid();
            var organismLocations = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { new Gatherer(guid, "DesignTimeOrganism-01", Colors.Silver), new Coordinate(0, 0) },
                                            { new Gatherer(guid, "DesignTimeOrganism-02", Colors.Silver), new Coordinate(EcosystemWidth - 1, EcosystemHeight - 1) }
                                        };

            return organismLocations;
        }
    }
}
