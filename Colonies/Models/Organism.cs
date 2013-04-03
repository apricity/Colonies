namespace Colonies.Models
{
    using System.Drawing;

    public sealed class Organism
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public Organism(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Color);
        }
    }
}