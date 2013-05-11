namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IStimuliProcessingLogic
    {
        List<Stimulus> ProcessStimuli(List<List<Stimulus>> biasedStimuli, Random random);
    }
}