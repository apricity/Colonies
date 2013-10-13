namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;

    public static class ColourLogic
    {
        // TODO: include pheromone?
        public static SolidColorBrush EnvironmentBrush(Color baseColor, WeightedColor mineral, List<WeightedColor> environmentModifiers)
        {
            // set up the standard default colour and modify it by how much mineral is available
            var environmentColor = baseColor;
            environmentColor = InterpolateColor(environmentColor, mineral.Color, mineral.Weight);

            // order the environment modifiers
            var orderedEnvironmentModifiers =
                environmentModifiers.Where(modifier => Math.Abs(modifier.Weight - 0.0) > 0.0)
                                    .OrderByDescending(modifier => modifier.Weight)
                                    .ToList();

            // if there are no brushes to apply, return the current terrain brush
            if (orderedEnvironmentModifiers.Count == 0)
            {
                return new SolidColorBrush(environmentColor);
            }

            // if there is only one brush to apply, do so and return the result
            if (orderedEnvironmentModifiers.Count == 1)
            {
                var environmentModifier = orderedEnvironmentModifiers.Single();
                environmentColor = InterpolateColor(environmentColor, environmentModifier.Color, environmentModifier.Weight);
                return new SolidColorBrush(environmentColor);
            }

            // calculate the color of the environment modifiers and 
            var environmentModifierOpacity = orderedEnvironmentModifiers.First().Weight;
            var environmentModifierColor = InterpolateWeightedColors(orderedEnvironmentModifiers);
            environmentColor = InterpolateColor(environmentColor, environmentModifierColor, environmentModifierOpacity);
            return new SolidColorBrush(environmentColor);
        }

        private static Color InterpolateColor(Color baseColor, Color modifyColor, double modifyRatio)
        {
            if (modifyRatio < 0 || modifyRatio > 1)
            {
                throw new ArgumentOutOfRangeException("modifyRatio");
            }

            var alphaDifference = modifyColor.A - baseColor.A;
            var redDifference = modifyColor.R - baseColor.R;
            var greenDifference = modifyColor.G - baseColor.G;
            var blueDifference = modifyColor.B - baseColor.B;

            return new Color
            {
                A = (byte)(baseColor.A + (alphaDifference * modifyRatio)),
                R = (byte)(baseColor.R + (redDifference * modifyRatio)),
                G = (byte)(baseColor.G + (greenDifference * modifyRatio)),
                B = (byte)(baseColor.B + (blueDifference * modifyRatio))
            };
        }

        private static Color InterpolateWeightedColors(List<WeightedColor> weightedColors)
        {
            var weightedRs = new List<WeightedColorChannel>();
            var weightedGs = new List<WeightedColorChannel>();
            var weightedBs = new List<WeightedColorChannel>();

            foreach (var weightedColor in weightedColors)
            {
                weightedRs.Add(weightedColor.WeightedR);
                weightedGs.Add(weightedColor.WeightedG);
                weightedBs.Add(weightedColor.WeightedB);
            }

            var interpolatedR = InterpolateColorChannels(weightedRs);
            var interpolatedG = InterpolateColorChannels(weightedGs);
            var interpolatedB = InterpolateColorChannels(weightedBs);

            return Color.FromRgb(interpolatedR, interpolatedG, interpolatedB);
        }

        private static byte InterpolateColorChannels(List<WeightedColorChannel> weightedColorChannels)
        {
            var orderedWeightedChannels = weightedColorChannels.OrderByDescending(weightedChannel => weightedChannel.ChannelValue).ToList();

            if (orderedWeightedChannels.Count == 2)
            {
                var firstWeightedChannel = orderedWeightedChannels[0];
                var secondWeightedChannel = orderedWeightedChannels[1];

                var byteDistance = firstWeightedChannel.ChannelValue - secondWeightedChannel.ChannelValue;
                var changeFactor = secondWeightedChannel.Weight / (firstWeightedChannel.Weight + secondWeightedChannel.Weight);
                var bytesToMove = byteDistance * changeFactor;
                var interpolatedChannelValue = (byte)(firstWeightedChannel.ChannelValue - bytesToMove);
                return interpolatedChannelValue;
            }

            var nextColourValues = new List<WeightedColorChannel>();
            for (var i = 0; i < orderedWeightedChannels.Count - 1; i++)
            {
                var firstWeightedChannel = orderedWeightedChannels[i];
                var secondWeightedChannel = orderedWeightedChannels[i + 1];

                var byteDistance = firstWeightedChannel.ChannelValue - secondWeightedChannel.ChannelValue;
                var changeFactor = secondWeightedChannel.Weight / (firstWeightedChannel.Weight + secondWeightedChannel.Weight);
                var bytesToMove = byteDistance * changeFactor;
                var interpolatedChannelValue = (byte)(firstWeightedChannel.ChannelValue - bytesToMove);

                var weightDistance = secondWeightedChannel.Weight - firstWeightedChannel.Weight;
                var weightToMove = weightDistance * changeFactor;
                var interpolatedWeight = firstWeightedChannel.Weight + weightToMove;

                nextColourValues.Add(new WeightedColorChannel(interpolatedChannelValue, interpolatedWeight));
            }

            return InterpolateColorChannels(nextColourValues);
        }
    }
}
