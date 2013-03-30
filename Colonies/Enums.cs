using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    public enum Terrain
    {
        Unknown = -1,
        Earth,
        Grass,
        Water,
        Fire
    }

    public enum ViewModelMessage
    {
        CheckIfModelHasChanged
    }
}
