namespace Wacton.Colonies
{
    using System.Collections.Generic;

    public static class ExtensionMethods
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
            var list = new List<T>();
            foreach (var element in twoDimensionalArray)
            {
                list.Add(element);
            }

            return list;
        }
    }
}
