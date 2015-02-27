namespace Wacton.Colonies.Habitat
{
    using Wacton.Colonies.Environment;
    using Wacton.Colonies.Organism;

    public interface IHabitat
    {
        bool ContainsOrganism();

        IEnvironment Environment { get; }

        IOrganism Organism { get; }
    }
}