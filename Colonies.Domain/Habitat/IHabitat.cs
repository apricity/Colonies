namespace Wacton.Colonies.Domain.Habitat
{
    using Wacton.Colonies.Domain.Environment;
    using Wacton.Colonies.Domain.Organism;

    public interface IHabitat
    {
        bool ContainsOrganism();

        IEnvironment Environment { get; }

        IOrganism Organism { get; }
    }
}