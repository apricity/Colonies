namespace Wacton.Colonies.Interfaces
{
    using System.Windows.Media;

    using Wacton.Colonies.Ancillary;

    public interface IOrganism : IMeasurable<OrganismMeasure>, IBiased<EnvironmentMeasure>
    {
        string Name { get; }

        Color Color { get; }

        bool IsAlive { get; }
    }
}