namespace Wacton.Colonies.Ancillary
{
    using System.Windows.Media;

    public struct WeightedColor
    {
        public readonly Color Color;
        public readonly double Weight;

        public WeightedColorChannel WeightedR
        {
            get
            {
                return new WeightedColorChannel(this.Color.R, this.Weight);
            }
        }

        public WeightedColorChannel WeightedG
        {
            get
            {
                return new WeightedColorChannel(this.Color.G, this.Weight);
            }
        }

        public WeightedColorChannel WeightedB
        {
            get
            {
                return new WeightedColorChannel(this.Color.B, this.Weight);
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

    public struct WeightedColorChannel
    {
        public readonly byte ChannelValue;
        public readonly double Weight;

        public WeightedColorChannel(byte channelValue, double weight)
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
