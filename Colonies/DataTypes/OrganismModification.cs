namespace Wacton.Colonies.DataTypes
{
    using Wacton.Colonies.DataTypes.Enums;
    
    public class OrganismModification
    {
        public Coordinate Coordinate { get; private set; }
        public OrganismMeasure Measure { get; private set; }
        public double Delta { get; private set; }

        public OrganismModification(Coordinate coordinate, OrganismMeasure measure, double delta)
        {
            this.Coordinate = coordinate;
            this.Measure = measure;
            this.Delta = delta;
        }

        public override string ToString()
        {
            return string.Format("Coordinate {0}: Organism {1} {2}", this.Coordinate, this.Measure, this.Delta);
        }
    }
}
