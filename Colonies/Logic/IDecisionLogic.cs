namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IDecisionLogic
    {
        List<Stimulus> MakeDecision(List<List<Stimulus>> biasedStimuli, Random random);
    }
}