namespace Wacton.Colonies.ViewModels
{
    using System.Text;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Interfaces;

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
                this.OnPropertyChanged("EnvironmentViewModel");
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
                this.OnPropertyChanged("OrganismViewModel");
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
                this.OnPropertyChanged("ToolTip");
            }
        }

        public HabitatViewModel(IHabitat domainModel, EnvironmentViewModel environmentViewModel, OrganismViewModel organismViewModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.EnvironmentViewModel = environmentViewModel;
            this.OrganismViewModel = organismViewModel;
        }

        public void RefreshEnvironment()
        {
            this.EnvironmentViewModel.Refresh();
        }

        public void RefreshOrganism()
        {
            this.OrganismViewModel.Refresh();
        }

        public void AssignOrganismModel(IOrganism model)
        {
            this.OrganismViewModel.AssignModel(model);
        }

        public void UnassignOrganismModel()
        {
            this.OrganismViewModel.UnassignModel();
        }

        public void RefreshToolTip()
        {
            var stringBuilder = new StringBuilder();
            foreach (var measurement in this.DomainModel.Environment.MeasurementData.Measurements)
            {
                stringBuilder.AppendLine(string.Format("{0}: {1:0.000}", measurement.Measure, measurement.Level));
            }

            if (this.DomainModel.ContainsOrganism())
            {
                stringBuilder.AppendLine("----------");
                stringBuilder.AppendLine(this.DomainModel.Organism.Name);

                foreach (var measurement in this.DomainModel.Organism.MeasurementData.Measurements)
                {
                    stringBuilder.AppendLine(string.Format("{0}: {1:0.000}", measurement.Measure, measurement.Level));
                }
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            this.ToolTip = stringBuilder.ToString();
        }

        public override void Refresh()
        {
            // refresh child view models (environment & organism)
            this.RefreshEnvironment();
            this.RefreshOrganism();
            this.RefreshToolTip();
        }
    }
}
