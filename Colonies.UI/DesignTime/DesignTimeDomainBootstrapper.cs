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
    using Wacton.Colonies.Domain.Plugins;

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

        protected override List<ColonyPluginData> InitialColonyData(PluginImporter pluginImporter)
        {
            // visual studio seems to lock DLLs that are reflection-loaded due to calls from the designer
            // so overriding the plugin loading behaviour at design-time to make development easier
            return new List<ColonyPluginData>();
        }

        protected override Dictionary<IOrganism, Coordinate> InitialOrganismCoordinates(OrganismFactory organismFactory)
        {
            var organism1 = organismFactory.CreateDummyOrganism(Colors.Silver);
            var organism2 = organismFactory.CreateDummyOrganism(Colors.Silver);

            var organismLocations = new Dictionary<IOrganism, Coordinate>
                                        {
                                            { organism1, new Coordinate(0, 0) },
                                            { organism2, new Coordinate(EcosystemWidth - 1, EcosystemHeight - 1) }
                                        };

            return organismLocations;
        }
    }
}
