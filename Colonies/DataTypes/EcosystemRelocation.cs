namespace Wacton.Colonies.DataTypes
{
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemRelocation
    {
        public IOrganism Organism { get; private set; }
        public Coordinate PreviousCoordinate { get; private set; }
        public Coordinate UpdatedCoordinate { get; private set; }

        public EcosystemRelocation(IOrganism organism, Coordinate previousCoordinate, Coordinate updatedCoordinate)
        {
            this.Organism = organism;
            this.PreviousCoordinate = previousCoordinate;
            this.UpdatedCoordinate = updatedCoordinate;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}->{2}", this.Organism.Name, this.PreviousCoordinate, this.UpdatedCoordinate);
        }
    }
}
