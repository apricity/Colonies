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

        // TODO: this should be a method that calculates an INTENTION based on what it knows (from parameters)
        public Habitat TakeTurn(List<Habitat> nearbyHabitats)
        {
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Color);
        }
    }
}