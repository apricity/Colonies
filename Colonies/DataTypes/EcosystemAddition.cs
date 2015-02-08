﻿namespace Wacton.Colonies.DataTypes
{
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemAddition
    {
        public IOrganism Organism { get; private set; }
        public Coordinate Coordinate { get; private set; }

        public EcosystemAddition(IOrganism organism, Coordinate coordinate)
        {
            this.Organism = organism;
            this.Coordinate = coordinate;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Organism.Name, this.Coordinate);
        }
    }
}