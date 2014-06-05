namespace Wacton.Colonies.Models.Interfaces
{
    public interface IHabitat
    {
        bool ContainsOrganism();

        IEnvironment Environment { get; }

        IOrganism Organism { get; }
    }
}