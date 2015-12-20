namespace Wacton.Colonies.UI.Organisms
{
    using System.Collections.Generic;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Organisms;
    using Wacton.Colonies.UI.Infrastructure;
    using Wacton.Tovarisch.Color;

    public class OrganismViewModel : ViewModelBase<IOrganism>
    {
        private static readonly Dictionary<Inventory, Color> InventoryColors =
            new Dictionary<Inventory, Color>
                {
                    { Inventory.None, Colors.Transparent },
                    { Inventory.Mineral, Colors.Goldenrod },
                    { Inventory.Nutrient, Colors.OliveDrab },
                    { Inventory.Spawn, Colors.Black }
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
                this.OnPropertyChanged(nameof(this.HasOrganism));
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
                this.OnPropertyChanged(nameof(this.Color));
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
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        private string nameAndIntention;
        public string NameAndIntention
        {
            get
            {
                return this.nameAndIntention;
            }
            set
            {
                this.nameAndIntention = value;
                this.OnPropertyChanged(nameof(this.NameAndIntention));
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
                this.OnPropertyChanged(nameof(this.HealthLevel));
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
                this.OnPropertyChanged(nameof(this.IsAlive));
            }
        }

        private Color inventoryColor;
        public Color InventoryColor
        {
            get
            {
                return this.inventoryColor;
            }
            set
            {
                this.inventoryColor = value;
                this.OnPropertyChanged(nameof(this.InventoryColor));
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
                this.OnPropertyChanged(nameof(this.InventoryScalar));
            }
        }

        public static double HabitatScale => 0.5;

        private Color hazardColor;
        public Color HazardColor
        {
            get
            {
                return this.hazardColor;
            }
            set
            {
                this.hazardColor = value;
                this.OnPropertyChanged(nameof(this.HazardColor));
            }
        }
        
        public OrganismViewModel(IOrganism domainModel, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
        }

        // TODO: can Refresh() be a generic ViewModel method?
        public override void Refresh()
        {
            if (this.DomainModel == null)
            {
                return;
            }

            this.HasOrganism = true;
            this.Color = this.DomainModel.Color;
            this.IsAlive = this.DomainModel.IsAlive;
            this.HealthLevel = this.DomainModel.MeasurementData.GetLevel(OrganismMeasure.Health);
            this.Name = this.DomainModel.Name;
            this.NameAndIntention = $"{this.DomainModel.Name} : {this.DomainModel.Age.ToString("0.00")} ({this.DomainModel.Description} | {this.DomainModel.CurrentIntention})";
            this.InventoryColor = InventoryColors[this.DomainModel.CurrentInventory];
            this.InventoryScalar = this.DomainModel.GetLevel(OrganismMeasure.Inventory) / 2.0;
            this.RefreshHazardColor();
        }

        private void RefreshHazardColor()
        {
            this.HazardColor = ColorLogic.ModifyColor(
                Colors.White,
                new List<WeightedColor>
                    {
                        new WeightedColor(Colors.CornflowerBlue, this.DomainModel.IsSoundOverloaded ? 1.0 : 0.0),
                        new WeightedColor(Colors.Tomato, this.DomainModel.IsPheromoneOverloaded ? 1.0 : 0.0),
                        new WeightedColor(Colors.YellowGreen, this.DomainModel.IsDiseased || this.DomainModel.IsInfectious ? 1.0 : 0.0)
                    });
        }
    }
}
