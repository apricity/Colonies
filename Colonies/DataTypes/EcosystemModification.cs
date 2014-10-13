﻿namespace Wacton.Colonies.DataTypes
{
    using Wacton.Colonies.DataTypes.Interfaces;

    public class EcosystemModification
    {
        public Coordinate Coordinate { get; private set; }
        public IMeasure Measure { get; private set; }
        public double PreviousLevel { get; private set; }
        public double UpdatedLevel { get; private set; }

        public double Delta
        {
            get
            {
                // TODO: rounding?
                return this.UpdatedLevel - this.PreviousLevel;
            }
        }

        public EcosystemModification(Coordinate coordinate, IMeasure measure, double previousLevel, double updatedLevel)
        {
            this.Coordinate = coordinate;
            this.Measure = measure;
            this.PreviousLevel = previousLevel;
            this.UpdatedLevel = updatedLevel;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}->{3}", this.Coordinate, this.Measure, this.PreviousLevel, this.UpdatedLevel);
        }
    }
}