namespace Wacton.Colonies.Domain.Extensions
{
    using System.Collections.Generic;

    public static class GenericCollectionExtensions
    {
        public static int Width<T>(this T[,] twoDimensionalArray)
        {
            return twoDimensionalArray.GetLength(0);
        }

        public static int Height<T>(this T[,] twoDimensionalArray)
        {
            return twoDimensionalArray.GetLength(1);
        }

        public static List<T> ToList<T>(this T[,] twoDimensionalArray)
        {
            // note one-liner?: return twoDimensionalArray.Cast<T>().ToList();

            var list = new List<T>();
            foreach (var element in twoDimensionalArray)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
