namespace Wacton.Colonies.Ancillary
{
    using System.Windows.Media;

    public struct WeightedColor
    {
        public readonly Color Color;
        public readonly double Weight;

        public WeightedChannelValue WeightedR
        {
            get
            {
                return new WeightedChannelValue(this.Color.R, this.Weight);
            }
        }

        public WeightedChannelValue WeightedG
        {
            get
            {
                return new WeightedChannelValue(this.Color.G, this.Weight);
            }
        }

        public WeightedChannelValue WeightedB
        {
            get
            {
                return new WeightedChannelValue(this.Color.B, this.Weight);
            }
        }

        public WeightedColor(Color color, double weight)
        {
            this.Color = color;
            this.Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Color.ToString(), this.Weight);
        }
    }

    public struct WeightedChannelValue
    {
        public readonly byte ChannelValue;
        public readonly double Weight;

        public WeightedChannelValue(byte channelValue, double weight)
        {
            this.ChannelValue = channelValue;
            this.Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.ChannelValue, this.Weight);
        }
    }
}
