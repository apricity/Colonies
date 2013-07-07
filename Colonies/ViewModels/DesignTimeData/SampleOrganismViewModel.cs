namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleOrganismViewModel : OrganismViewModel
    {
        public SampleOrganismViewModel()
            : base(CreateSampleOrganism(), new EventAggregator())
        {
            
        }

        public Organism SampleOrganism
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Organism CreateSampleOrganism()
        {
            return new Organism("Sample", Colors.Black, true);
        }
    }
}
