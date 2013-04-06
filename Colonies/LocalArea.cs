using Colonies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    public class LocalArea
    {
        public List<Habitat> LocalHabitats { get; set; }
        public Habitat HabitatOfFocus { get; set; }

        public LocalArea (List<Habitat> localHabitats , Habitat habitatOfFocus)
        {
            this.LocalHabitats = localHabitats;
            this.HabitatOfFocus = habitatOfFocus;
        }

        public int Size()
        {
            return this.LocalHabitats.Count;
        }
    }
}
