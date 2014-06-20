namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;
    using Wacton.Colonies.ViewModels.Infrastructure;

    public class OrganismViewModel : ViewModelBase<IOrganism>
    {
        private static readonly Dictionary<EnvironmentMeasure, Color> InventoryColors =
            new Dictionary<EnvironmentMeasure, Color>
                {
                    { EnvironmentMeasure.Mineral, Colors.Goldenrod },
                    { EnvironmentMeasure.Nutrient, Colors.OliveDrab }
                };

        // do not set domain model properties through the view model
        // use events to tell view models the model has changed
        private bool hasOrganism;
        public bool HasOrganism
        {
            get
            {
                return this.hasOrganism;
            }
            set
            {
                this.hasOrganism = value;
                this.OnPropertyChanged("HasOrganism");
            }
        }

        private Color color;
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                this.OnPropertyChanged("Color");
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        private double healthLevel;
        public double HealthLevel
        {
            get
            {
                return this.healthLevel;
            }
            set
            {
                this.healthLevel = value;
                this.OnPropertyChanged("HealthLevel");
            }
        }

        private bool isAlive;
        public bool IsAlive
        {
            get
            {
                return this.isAlive;
            }
            set
            {
                this.isAlive = value;
                this.OnPropertyChanged("IsAlive");
            }
        }

        public Color inventoryColor;
        public Color InventoryColor
        {
            get
            {
                return this.inventoryColor;
            }
            set
            {
                this.inventoryColor = value;
                this.OnPropertyChanged("InventoryColor");
            }
        }

        private double inventoryScalar;
        public double InventoryScalar
        {
            get
            {
                return this.inventoryScalar;
            }
            set
            {
                this.inventoryScalar = value;
                this.OnPropertyChanged("InventoryScalar");
            }
        }

        public static double HabitatScale
        {
            get
            {
                return 0.5;
            }
        }
        
        public OrganismViewModel(IOrganism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {

        }

        // TODO: can Refresh() be a generic ViewModel method?
        public override void Refresh()
        {
            if (this.DomainModel != null)
            {
                this.HasOrganism = true;
                this.Color = this.DomainModel.Color;
                this.IsAlive = this.DomainModel.IsAlive;
                this.HealthLevel = this.DomainModel.MeasurementData.GetLevel(OrganismMeasure.Health);
                this.Name = this.DomainModel.Name;

                if (this.DomainModel.Inventory != null)
                {
                    this.InventoryColor = InventoryColors[(EnvironmentMeasure)this.DomainModel.Inventory.Measure];
                    this.InventoryScalar = this.DomainModel.Inventory.Level / 2.0;
                }
                else
                {
                    this.InventoryColor = default(Color);
                    this.InventoryScalar = default(double);
                }
            }
            else
            {
                // reset the properties to their default values if no organism in the model
                this.HasOrganism = false;
                this.Color = default(Color);
                this.IsAlive = default(bool);
                this.HealthLevel = default(double);
                this.Name = default(string);
                this.InventoryColor = default(Color);
                this.InventoryScalar = default(double);
            }
        }
    }
}
