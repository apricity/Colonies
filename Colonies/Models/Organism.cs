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

        // TODO: this should be a method that calculates an INTENTION based on factors (does not necessarily choose a feature to move to)
        public HabitatFactorSet TakeTurn(IEnumerable<HabitatFactorSet> possibleFactorSets, Random random)
        {
            // something like this - more desirable features will have greater weightings
            // except will be using 0.0 -> 1.0 range for strengths/levels
            var weightings = new List<HabitatFactorSet>();
            foreach (var factorSet in possibleFactorSets)
            {
                for (int i = 0; i < factorSet.Strength; i++)
                {
                    weightings.Add(factorSet);
                }
            }

            var decisionIndex = random.Next(weightings.Count);
            var chosenFactorSet = weightings[decisionIndex];

            return chosenFactorSet;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Color);
        }
    }
}