namespace Wacton.Colonies.Models.Interfaces
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

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

        bool CanInteractEnvironment(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        IntentionAdjustments InteractEnvironmentAdjustments(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        bool CanInteractOrganism();

        IntentionAdjustments InteractOrganismAdjustments(IOrganismState otherOrganismState);
    }
}