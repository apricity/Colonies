namespace Wacton.Colonies.DataTypes
{
    using Wacton.Colonies.DataTypes.Enums;

    public class EnvironmentModification
    {
        public Coordinate Coordinate { get; private set; }
        public EnvironmentMeasure Measure { get; private set; }
        public double Delta { get; private set; }

        public EnvironmentModification(Coordinate coordinate, EnvironmentMeasure measure, double delta)
        {
            this.Coordinate = coordinate;
            this.Measure = measure;
            this.Delta = delta;
        }

        public override string ToString()
        {
            return string.Format("Coordinate {0}: Environment {1} {2}", this.Coordinate, this.Measure, this.Delta);
        }
    }
}
