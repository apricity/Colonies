namespace Colonies.Models
{
    using System.Drawing;

    public sealed class Organism
    {
        public string OrganismID { get; private set; }
        public Color Color { get; private set; }

        public Organism(string organismID)
        {
            this.OrganismID = organismID;
            this.Color = Color.Black;
        }

        public Organism(string organismID, Color color)
        {
            this.OrganismID = organismID;
            this.Color = color;
        }

        public override string ToString()
        {
            return this.OrganismID;
        }
    }
}
