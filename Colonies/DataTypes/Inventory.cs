namespace Wacton.Colonies.DataTypes
{
    using System;

    using Wacton.Colonies.DataTypes.Enums;

    public class Inventory
    {
        public EnvironmentMeasure EnvironmentMeasure { get; private set; }
        public double Amount { get; private set; }

        public Inventory(EnvironmentMeasure environmentMeasure, double amount)
        {
            if (!environmentMeasure.IsTransportable)
            {
                throw new ArgumentException(string.Format("Environment measure {0} is not transportable", environmentMeasure));
            }

            this.EnvironmentMeasure = environmentMeasure;
            this.Amount = amount;
        }
    }
}
