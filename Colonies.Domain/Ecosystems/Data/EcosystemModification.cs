namespace Wacton.Colonies.Domain.Ecosystems.Data
{
    using Wacton.Colonies.Domain.Core;
    using Wacton.Colonies.Domain.Measures;

    public class EcosystemModification
    {
        public Coordinate Coordinate { get; }
        public IMeasure Measure { get; }
        public double PreviousLevel { get; }
        public double UpdatedLevel { get; }

        public double Delta => this.UpdatedLevel - this.PreviousLevel;

        public EcosystemModification(Coordinate coordinate, IMeasure measure, double previousLevel, double updatedLevel)
        {
            this.Coordinate = coordinate;
            this.Measure = measure;
            this.PreviousLevel = previousLevel;
            this.UpdatedLevel = updatedLevel;
        }

        public override string ToString() => $"{this.Coordinate}: {this.Measure} {this.PreviousLevel}->{this.UpdatedLevel}";
    }
}
