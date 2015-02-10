namespace Wacton.Colonies.DataTypes
{
    public struct WeightedItem<T>
    {
        public readonly T Item;
        public readonly double Weight;

        public WeightedItem(T item, double weight)
        {
            this.Item = item;
            this.Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Item, this.Weight);
        }
    }
}
