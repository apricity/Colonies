namespace Wacton.Colonies.ViewModels.DesignTimeData
{
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;

    public sealed class DesignTimeOrganismViewModel : OrganismViewModel, IDesignTimeViewModel<Organism>
    {
        public DesignTimeOrganismViewModel()
            : base(CreateDesignTimeOrganism(), new EventAggregator())
        {
            
        }

        public Organism DesignTimeModel
        {
            get
            {
                return this.DomainModel;
            }
        }

        private static Organism CreateDesignTimeOrganism()
        {
            return new Organism("Sample", Colors.Gray);
        }
    }
}
