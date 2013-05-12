using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies.Models
{
    public interface IMeasurable
    {
        List<Measurement> GetMeasurements();
    }
}
