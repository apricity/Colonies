namespace Wacton.Colonies.Domain.Ecosystems.Data
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Organisms;

    public class EcosystemRelocation
    {
        public IOrganism Organism { get; }
        public Coordinate PreviousCoordinate { get; }
        public Coordinate UpdatedCoordinate { get; }

        public EcosystemRelocation(IOrganism organism, Coordinate previousCoordinate, Coordinate updatedCoordinate)
        {
            this.Organism = organism;
            this.PreviousCoordinate = previousCoordinate;
            this.UpdatedCoordinate = updatedCoordinate;
        }

        public override string ToString() => $"{this.Organism.Name} {this.PreviousCoordinate}->{this.UpdatedCoordinate}";
    }
}
