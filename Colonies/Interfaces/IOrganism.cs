namespace Wacton.Colonies.Interfaces
{
    using System.Windows.Media;

    public interface IOrganism : IMeasurable, IBiased
    {
        string Name { get; }

        Color Color { get; }

        bool IsAlive { get; }
    }
}