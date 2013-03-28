using System;
using System.Collections.Generic;
using System.Linq;

namespace Colonies
{
    public sealed class Ecosystem
    {
        public List<List<Niche>> Niches { get; set; }

        // TODO: are height and width the right way round?
        public int Height
        {
            get
            {
                return this.Niches.Count;
            }
        }

        public int Width
        {
            get
            {
                return this.Niches.First().Count;
            }
        }

        public Ecosystem(List<List<Niche>> niches)
        {
            this.Niches = niches;
        }

        public override String ToString()
        {
            return this.Width + " x " + this.Height;
        }
    }
}
