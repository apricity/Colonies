namespace Colonies.Logic
{
    using System;
    using System.Collections.Generic;

    using Colonies.Models;

    public interface IStimuliProcessingLogic
    {
        Stimulus ProcessStimuli(IEnumerable<Stimulus> stimuli, double pheromoneWeighting, Random random);
    }
}