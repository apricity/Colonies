namespace Wacton.Colonies.UI.Habitats
{
    using System.Text;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Habitats;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.UI.Environments;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Colonies.UI.Organisms;

    public class HabitatViewModel : ViewModelBase<IHabitat>
    {
        private EnvironmentViewModel environmentViewModel;
        public EnvironmentViewModel EnvironmentViewModel
        {
            get
            {
                return this.environmentViewModel;
            }
            set
            {
                this.environmentViewModel = value;
                this.OnPropertyChanged(nameof(this.EnvironmentViewModel));
            }
        }

        private OrganismViewModel organismViewModel;
        public OrganismViewModel OrganismViewModel
        {
            get
            {
                return this.organismViewModel;
            }
            set
            {
                this.organismViewModel = value;
                this.OnPropertyChanged(nameof(this.OrganismViewModel));
            }
        }

        private string toolTip;
        public string ToolTip
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                this.toolTip = value;
                this.OnPropertyChanged(nameof(this.ToolTip));
            }
        }

        public HabitatViewModel(IHabitat domainModel, EnvironmentViewModel environmentViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EnvironmentViewModel = environmentViewModel;
            this.OrganismViewModel = new OrganismViewModel(domainModel.Organism, this.EventAggregator);
        }

        public void AssignOrganismModel(IOrganism model)
        {
            this.OrganismViewModel = new OrganismViewModel(model, this.EventAggregator);
        }

        public void UnassignOrganismModel()
        {
            this.OrganismViewModel = new OrganismViewModel(null, this.EventAggregator);
        }

        private void RefreshToolTip()
        {
            var stringBuilder = new StringBuilder();
            foreach (var measurement in this.DomainModel.Environment.MeasurementData.Measurements)
            {
                stringBuilder.AppendLine($"{measurement.Measure}: {measurement.Level:0.000}");
            }

            if (this.DomainModel.ContainsOrganism())
            {
                stringBuilder.AppendLine("----------");
                stringBuilder.AppendLine(this.DomainModel.Organism.Name);
                stringBuilder.AppendLine($"Intention: {this.DomainModel.Organism.CurrentIntention} ({this.DomainModel.Organism.CurrentInventory})");

                foreach (var measurement in this.DomainModel.Organism.MeasurementData.Measurements)
                {
                    stringBuilder.AppendLine($"{measurement.Measure}: {measurement.Level:0.000}");
                }

                stringBuilder.AppendLine($"Pheromone {(this.DomainModel.Organism.IsPheromoneOverloaded ? "overloaded" : "normal")}");
                stringBuilder.AppendLine($"Sound {(this.DomainModel.Organism.IsSoundOverloaded ? "overloaded" : "normal")}");
                stringBuilder.AppendLine(this.DomainModel.Organism.IsDiseased ? "Diseased" : "Not diseased");
                stringBuilder.AppendLine(this.DomainModel.Organism.IsInfectious ? "Infectious" : "Not infectious");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            this.ToolTip = stringBuilder.ToString();
        }

        public override void Refresh()
        {
            // refresh child view models (environment & organism)
            this.EnvironmentViewModel.Refresh();
            this.OrganismViewModel.Refresh();
            this.RefreshToolTip();
        }
    }
}
