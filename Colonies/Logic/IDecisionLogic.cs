namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IDecisionLogic
    {
        List<Measurement> MakeDecision(List<List<Measurement>> measurementsCollection, Random random);
    }
}