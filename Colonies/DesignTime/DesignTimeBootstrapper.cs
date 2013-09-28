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
            // do nothing (leave all terrain as earth default)
        }

        // TODO: tidy up how this is done in the bootstrapper
        protected override Dictionary<Organism, Coordinates> InitialiseOrganisms(Ecosystem ecosystem)
        {
            var organismLocations = new Dictionary<Organism, Coordinates>
                                        {
                                            { new Organism("DesignTimeOrganism-01", Colors.Silver), new Coordinates(0, 0) },
                                            { new Organism("DesignTimeOrganism-02", Colors.Silver), new Coordinates(EcosystemWidth - 1, EcosystemHeight - 1) }
                                        };

            foreach (var organismLocation in organismLocations)
            {
                ecosystem.AddOrganism(organismLocation.Key, organismLocation.Value);
            }

            return organismLocations;
        }
    }
}
