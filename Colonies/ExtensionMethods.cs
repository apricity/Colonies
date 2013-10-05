namespace Wacton.Colonies
{
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
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
