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

        // TODO: this should be a method that calculates an INTENTION based on conditions (does not necessarily choose a condition to move to)
        public HabitatCondition TakeTurn(IEnumerable<HabitatCondition> possibleHabitatConditions, Random random)
        {
            // something like this - more desirable features will have greater weightings
            // except will be using 0.0 -> 1.0 range for strengths/levels
            var weightedHabitatConditions = new List<HabitatCondition>();
            foreach (var habitatCondition in possibleHabitatConditions)
            {
                for (int i = 0; i < habitatCondition.Strength; i++)
                {
                    weightedHabitatConditions.Add(habitatCondition);
                }
            }

            var decisionIndex = random.Next(weightedHabitatConditions.Count);
            var chosenHabitatCondition = weightedHabitatConditions[decisionIndex];

            return chosenHabitatCondition;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Color);
        }
    }
}