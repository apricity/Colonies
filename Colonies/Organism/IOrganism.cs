namespace Wacton.Colonies.Organism
{
    using System.Windows.Media;

    using Wacton.Colonies.Intentions;
    using Wacton.Colonies.Measures;

    public interface IOrganism : IOrganismState, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        double Age { get; }

        void IncrementAge(double increment);

        Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        void UpdateIntention(Intention intention);

        void OverloadPheromone();

        void OverloadSound();

        void ContractDisease();

        bool CanAct(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        IntentionAdjustments ActionEffects(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        bool CanInteract();

        IntentionAdjustments InteractionEffects(IOrganismState otherOrganismState);
    }
}