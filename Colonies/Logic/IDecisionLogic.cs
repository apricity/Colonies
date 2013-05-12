namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IDecisionLogic
    {
        Measurement MakeDecision(List<Measurement> measurements, Random random);
    }
}