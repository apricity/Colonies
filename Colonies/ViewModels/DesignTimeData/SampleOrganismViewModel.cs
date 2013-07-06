namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class SampleOrganismViewModel : OrganismViewModel
    {
        public SampleOrganismViewModel()
            : this(CreateSampleOrganism(), new EventAggregator())
        {
            
        }

        public SampleOrganismViewModel(Organism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }

        private static Organism CreateSampleOrganism()
        {
            return new Organism("Sample", Colors.CornflowerBlue, true);
        }
    }
}
