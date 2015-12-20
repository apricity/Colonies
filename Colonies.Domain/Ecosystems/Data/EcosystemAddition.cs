namespace Wacton.Colonies.Domain.Ecosystems.Data
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Organisms;

    public class EcosystemAddition
    {
        public IOrganism Organism { get; }
        public Coordinate Coordinate { get; }

        public EcosystemAddition(IOrganism organism, Coordinate coordinate)
        {
            this.Organism = organism;
            this.Coordinate = coordinate;
        }

        public override string ToString() => $"{this.Organism.Name} {this.Coordinate}";
    }
}
