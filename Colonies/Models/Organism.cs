namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Colonies.Logic;

    public sealed class Organism
    {
        // TODO: this should not be hard-coded, perhaps
        private const int PheromoneWeighting = 10;

        public string Name { get; private set; }
        public Color Color { get; private set; }
        public double Health { get; private set; }
        public bool IsDepositingPheromones { get; private set; }

        private readonly IStimuliProcessingLogic stimuliProcessingLogic;

        public Organism(string name, Color color, IStimuliProcessingLogic stimuliProcessingLogic, bool isDepostingPheromones)
        {
            this.Name = name;
            this.Color = color;
            this.stimuliProcessingLogic = stimuliProcessingLogic;
            this.Health = 1.0;

            // TODO: depositing pheromones should probably not be something that is handled through construction (it will probably be very dynamic)
            this.IsDepositingPheromones = isDepostingPheromones;
        }

        public void DecreaseHealth(double decreaseLevel)
        {
            this.Health -= decreaseLevel;
            if (this.Health < 0)
            {
                this.Health = 0;
            }
        }

        public List<Stimulus> ProcessStimuli(List<List<Stimulus>> stimuli, Random random)
        {
            foreach (var stimulus in stimuli.SelectMany(stimulusSet => stimulusSet))
            {
                stimulus.SetBias(PheromoneWeighting);
            }

            var chosenBiasedStimulus = this.stimuliProcessingLogic.ProcessStimuli(stimuli, random);
            return chosenBiasedStimulus;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", this.Name, this.Health * 100, this.Color);
        }
    }
}