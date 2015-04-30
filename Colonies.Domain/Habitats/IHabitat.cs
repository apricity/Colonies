namespace Wacton.Colonies.Domain.Habitats
{
    using Wacton.Colonies.Domain.Environments;
    using Wacton.Colonies.Domain.Organisms;

    public interface IHabitat
    {
        bool ContainsOrganism();

        IEnvironment Environment { get; }

        IOrganism Organism { get; }
    }
}