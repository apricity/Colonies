namespace Colonies.Models
{
    using System.Drawing;

    public sealed class Organism
    {
        public string OrganismID { get; set; }
        public Color Color { get; set; }

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
            return string.Format("{0} ({1})", this.OrganismID, this.Color);
        }
    }
}