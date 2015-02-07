namespace Wacton.Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Microsoft.Practices.Prism.PubSubEvents;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.Interfaces;
    using Wacton.Colonies.ViewModels.Infrastructure;

    public class EcosystemViewModel : ViewModelBase<IEcosystem>
    {
        private List<List<HabitatViewModel>> habitatViewModels;
        public List<List<HabitatViewModel>> HabitatViewModels
        {
            get
            {
                return this.habitatViewModels;
            }
            set
            {
                this.habitatViewModels = value;
                this.OnPropertyChanged("HabitatViewModels");
            }
        }

        private Color weatherColor;
        public Color WeatherColor
        {
            get
            {
                return this.weatherColor;
            }
            set
            {
                this.weatherColor = value;
                this.OnPropertyChanged("WeatherColor");
            }
        }

        public EcosystemViewModel(IEcosystem domainModel, List<List<HabitatViewModel>> habitatViewModels, IEventAggregator eventAggregator)
            : base(domainModel, eventAggregator)
        {
            this.HabitatViewModels = habitatViewModels;
        }

        public override void Refresh()
        {
            // refresh all child view models (each habitat)
            foreach (var habitatViewModel in this.HabitatViewModels.SelectMany(habitatViewModel => habitatViewModel))
            {
                habitatViewModel.Refresh();
            }

            this.RefreshWeatherColor();
        }

        public void RefreshWeatherColor()
        {
            this.WeatherColor = ColorLogic.WeatherColor(
                Color.FromRgb(17, 17, 17),
                new List<WeightedColor>
                    {
                        new WeightedColor(Colors.CornflowerBlue, this.DomainModel.Weather.GetLevel(WeatherType.Damp)),
                        new WeightedColor(Colors.Tomato, this.DomainModel.Weather.GetLevel(WeatherType.Heat))
                    });
        }
    }
}
