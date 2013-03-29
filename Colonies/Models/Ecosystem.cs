namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Ecosystem
    {
        public List<List<Habitat>> Habitats { get; set; }

        // TODO: are height and width the right way round?
        public int Height
        {
            get
            {
                return this.Habitats.Count;
            }
        }

        public int Width
        {
            get
            {
                return this.Habitats.First().Count;
            }
        }

        public Ecosystem(List<List<Habitat>> habitats)
        {
            this.Habitats = habitats;
        }

        public override String ToString()
        {
            return this.Width + " x " + this.Height;
        }
    }
}
