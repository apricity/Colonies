namespace Wacton.Colonies.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    public static class ColourLogic
    {
        public static SolidColorBrush TerrainBrush(Dictionary<SolidColorBrush, double> elements, SolidColorBrush terrainBrush)
        {
            var orderedElements =
                elements.Where(pair => Math.Abs(pair.Value - 0.0) > 0.0)
                        .OrderByDescending(pair => pair.Value)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);

            // if there are no brushes to apply, return the current terrain brush
            if (orderedElements.Count == 0)
            {
                return terrainBrush;
            }

            // if there is only one brush to apply, do so and return the result
            if (orderedElements.Count == 1)
            {
                return ModifyBrush(terrainBrush, orderedElements.Single().Key, orderedElements.Single().Value);
            }

            // if there are multiple brushes to apply
            // calculate the blended element colour using their respective ratio
            // then apply the blended colour, using the highest ratio of the elements
            var blendedRatio = orderedElements.First().Value;

            var redTuple = new List<Tuple<byte, double>>();
            var blueTuple = new List<Tuple<byte, double>>();
            var greenTuple = new List<Tuple<byte, double>>();
            foreach (var element in elements)
            {
                redTuple.Add(new Tuple<byte, double>(element.Key.Color.R, element.Value));
                greenTuple.Add(new Tuple<byte, double>(element.Key.Color.G, element.Value));
                blueTuple.Add(new Tuple<byte, double>(element.Key.Color.B, element.Value));
            }

            var red = InterpolateColourComponent(redTuple);
            var green = InterpolateColourComponent(greenTuple);
            var blue = InterpolateColourComponent(blueTuple);

            var elementBrush = new SolidColorBrush(Color.FromRgb(red, green, blue));

            terrainBrush = ModifyBrush(terrainBrush, elementBrush, blendedRatio);
            return terrainBrush;
        }

        public static byte InterpolateColourComponent(List<Tuple<byte, double>> weightedComponents)
        {
            var orderedWeightedComponents = weightedComponents.OrderByDescending(value => value.Item1).ToList();

            if (orderedWeightedComponents.Count == 2)
            {
                var firstWeightedComponent = orderedWeightedComponents[0];
                var secondWeightedComponent = orderedWeightedComponents[1];

                var colourDistance = firstWeightedComponent.Item1 - secondWeightedComponent.Item1;
                var distanceToMove = secondWeightedComponent.Item2 / (firstWeightedComponent.Item2 + secondWeightedComponent.Item2);
                var colourToMove = colourDistance * distanceToMove;
                var colour = firstWeightedComponent.Item1 - colourToMove;
                return (byte)colour;
            }

            var nextColourValues = new List<Tuple<byte, double>>();
            for (var i = 0; i < orderedWeightedComponents.Count - 1; i++)
            {
                var firstWeightedComponent = orderedWeightedComponents[i];
                var secondWeightedComponent = orderedWeightedComponents[i + 1];

                var colourDistance = firstWeightedComponent.Item1 - secondWeightedComponent.Item1;
                var changeFactor = secondWeightedComponent.Item2 / (firstWeightedComponent.Item2 + secondWeightedComponent.Item2);
                var colourToMove = colourDistance * changeFactor;
                var colour = firstWeightedComponent.Item1 - colourToMove;

                var weightDistance = secondWeightedComponent.Item2 - firstWeightedComponent.Item2;
                var weightToMove = weightDistance * changeFactor;
                var weight = firstWeightedComponent.Item2 + weightToMove;

                nextColourValues.Add(new Tuple<byte, double>((byte)colour, weight));
            }

            return InterpolateColourComponent(nextColourValues);
        }

        private static SolidColorBrush ModifyBrush(SolidColorBrush baseBrush, SolidColorBrush modifyBrush, double modifyRatio)
        {
            var modifiedColor = Interpolate(baseBrush.Color, modifyBrush.Color, modifyRatio);
            return new SolidColorBrush(modifiedColor);
        }

        private static Color Interpolate(Color baseColor, Color modifyColor, double modifyRatio)
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
    }
}
