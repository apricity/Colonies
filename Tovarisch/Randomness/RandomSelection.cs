namespace Wacton.Tovarisch.Randomness
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class RandomSelection
    {
        public static T SelectOne<T>(IEnumerable<T> items)
        {
            var chosenItem = default(T);
            var itemList = items.ToList();

            var itemChosen = false;
            var randomNumber = RandomNumberGenerator.DoubleBetween(itemList.Count);
            foreach (var item in itemList)
            {
                if (randomNumber <= 1)
                {
                    chosenItem = item;
                    itemChosen = true;
                    break;
                }

                randomNumber -= 1;
            }

            if (!itemChosen)
            {
                throw new NullReferenceException("An item has not been chosen");
            }

            return chosenItem;
        }

        public static T SelectOne<T>(IEnumerable<WeightedItem<T>> weightedItems)
        {
            var chosenItem = default(T);
            var itemList = weightedItems.ToList();
            var totalWeight = itemList.Sum(weightedItem => weightedItem.Weight);

            var itemChosen = false;
            var randomNumber = RandomNumberGenerator.DoubleBetween(totalWeight);
            foreach (var weightedItem in itemList)
            {
                if (randomNumber <= weightedItem.Weight)
                {
                    chosenItem = weightedItem.Item;
                    itemChosen = true;
                    break;
                }

                randomNumber -= weightedItem.Weight;
            }

            if (!itemChosen)
            {
                throw new NullReferenceException("An item has not been chosen");
            }

            return chosenItem;
        }

        public static bool IsSuccessful(double successProbability)
        {
            if (successProbability < 0 || successProbability > 1)
            {
                throw new ArgumentOutOfRangeException("successProbability", successProbability, "Success probability must be between 0 - 1");
            }

            if (successProbability == 0.0)
            {
                return false;
            }

            var randomNumber = RandomNumberGenerator.DoubleBetween(1);
            return randomNumber <= successProbability;
        }
    }
}
