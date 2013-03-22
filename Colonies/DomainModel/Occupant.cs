using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    public class Occupant
    {
        private readonly string occupantID;

        public Occupant(string occupantID)
        {
            this.occupantID = occupantID;
        }

        public new string ToString()
        {
            return this.occupantID;
        }
    }
}
