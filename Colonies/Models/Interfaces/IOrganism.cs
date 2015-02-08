namespace Wacton.Colonies.Models.Interfaces
{
    using System.Windows.Media;

    using Wacton.Colonies.DataTypes.Enums;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        double Age { get; }

        Inventory Inventory { get; }

        Intention Intention { get; }

        bool IsAlive { get; }

        bool IsReproductive { get; }

        bool IsAudible { get; }

        bool IsPheromoneOverloaded { get; }

        bool IsSoundOverloaded { get; }

        bool IsDiseased { get; }

        bool IsInfectious { get; }

        void IncrementAge(double increment);

        Intention DecideIntention(IMeasurable<EnvironmentMeasure> measurableEnvironment);

        void UpdateIntention(Intention intention);

        void OverloadPheromone();

        void OverloadSound();

        void ContractDisease();
    }
}