namespace Wacton.Tovarisch.Strings
{
    using System.Globalization;

    public static class StringExtensions
    {
        public static string ToTitleCase(this string value)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLower());
        }
    }
}
