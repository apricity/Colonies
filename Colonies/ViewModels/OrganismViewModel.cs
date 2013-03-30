namespace Colonies.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Media;

    using Colonies.Annotations;
    using Colonies.Models;

    using Microsoft.Practices.Prism.Events;

    public sealed class OrganismViewModel : ViewModelBase<Organism>
    {
        public SolidColorBrush OrganismBrush
        {
            get
            {
                var mediaColor = System.Windows.Media.Color.FromRgb(this.DomainModel.Color.R, this.DomainModel.Color.G, this.DomainModel.Color.B);
                return new SolidColorBrush(mediaColor);
            }
        }

        public OrganismViewModel(Organism model, IEventAggregator eventAggregator)
            : base(model, eventAggregator)
        {
            
        }
    }
}
