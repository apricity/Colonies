namespace Wacton.Colonies.DesignTime
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Models;
    using Wacton.Colonies.ViewModels;

    public class DesignTimeBootstrapper : Bootstrapper
    {
        private const int EcosystemWidth = 10;
        private const int EcosystemHeight = 10;

        public MainViewModel MainViewModel { get; private set; }

        public DesignTimeBootstrapper()
        {
            
        }

        public override void Run()
        {
            this.MainViewModel = this.BuildMainDataContext(EcosystemWidth, EcosystemHeight);
            this.MainViewModel.Refresh();
        }

        protected override void InitialiseTerrain(Ecosystem ecosystem)
        {
            ecosystem.Habitats[1, 1].Environment.SetLevel(Measure.Obstruction, 1.0);
        }

        // TODO: tidy up how this is done in the bootstrapper
        protected override Dictionary<Organism, Coordinate> InitialiseOrganisms(Ecosystem ecosystem)
        {
            var organismLocations = new Dictionary<Organism, Coordinate>
                                        {
                                            { new Organism("DesignTimeOrganism-01", Colors.Silver), new Coordinate(0, 0) },
                                            { new Organism("DesignTimeOrganism-02", Colors.Silver), new Coordinate(EcosystemWidth - 1, EcosystemHeight - 1) }
                                        };

            foreach (var organismLocation in organismLocations)
            {
                ecosystem.AddOrganism(organismLocation.Key, organismLocation.Value);
            }

            return organismLocations;
        }
    }
}
