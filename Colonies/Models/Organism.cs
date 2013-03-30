namespace Colonies.Models
{
    using System.ComponentModel;
    using System.Drawing;

    using Colonies.Annotations;

    public sealed class Organism
    {
        private readonly string organismID;
        public Color Color { get; private set; }

        public Organism(string organismID)
        {
            this.organismID = organismID;
            this.Color = Color.Black;
        }

        public Organism(string organismID, Color color)
        {
            this.organismID = organismID;
            this.Color = color;
        }

        public override string ToString()
        {
            return this.organismID;
        }
    }
}
