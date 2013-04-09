namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public sealed class Organism
    {
        private string Name { get; set; }
        public Color Color { get; set; }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        // TODO: this should be a method that calculates an INTENTION based on features (does not necessarily choose a feature to move to)
        public Features TakeTurn(IEnumerable<Features> nearbyFeatures, Random random)
        {
            // something like this - more desirable features will have greater weightings
            // except will be using 0.0 -> 1.0 range for strengths/levels
            var weightings = new List<Features>();
            foreach (var features in nearbyFeatures)
            {
                for (int i = 0; i < features.Strength; i++)
                {
                    weightings.Add(features);
                }
            }

            var decisionIndex = random.Next(weightings.Count);
            var chosenFeature = weightings[decisionIndex];

            return chosenFeature;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Color);
        }
    }
}